using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class PortfolioService(
        IFundamentalScoreService fundamentalScoreService,
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository,
        ILifePortfolioPositionRepository lifePortfolioPositionRepository,
        IInstrumentService instrumentService,
        IDataService dataService,
        IStorageApiClient storageApiClient)
        : IPortfolioService
    {
        /// <inheritdoc />
        public async Task<EditPortfolioPositionResponse> EditPortfolioPositionAsync(EditPortfolioPositionRequest request)
        {
            var instrument = await instrumentRepository.GetInstrumentByTickerAsync(request.Ticker);
            instrument!.ManualCoefficient = request.ManualCoefficient;
            await instrumentRepository.EditInstrumentAsync(instrument);

            var lifePortfolioPositions = await lifePortfolioPositionRepository.GetLifePortfolioPositionsAsync();

            var lifePortfolioPosition = lifePortfolioPositions.Find(x => x.Ticker == request.Ticker);

            if (lifePortfolioPosition is null)
                await lifePortfolioPositionRepository.AddLifePortfolioPositionAsync(
                    new Core.Models.LifePortfolioPosition
                    {
                        Ticker = request.Ticker,
                        Name = instrument.Name,
                        Size = request.LifeSize,
                        IsDeleted = false
                    });

            else
                await lifePortfolioPositionRepository.EditSizeLifePortfolioPositionAsync(request.Ticker, request.LifeSize);

            return new();
        }

        /// <inheritdoc />
        public async Task<EditPortfolioTotalSumResponse> EditPortfolioTotalSumAsync(EditPortfolioTotalSumRequest request)
        {
            await parameterRepository.SetParameterValueAsync(KnownParameters.TotalSum, request.TotalSum.ToString("N0"));
            return new();
        }

        /// <inheritdoc />
        public async Task<PortfolioApplyWeightResponse> PortfolioApplyWeightAsync(PortfolioApplyWeightRequest request)
        {
            var portfolioPositionResponse = (await GetPortfolioPositionListAsync(new()));

            foreach (var position in portfolioPositionResponse.PortfolioPositions)
            {
                var lifePercent = position.LifeSize * position.Price / portfolioPositionResponse.TotalSum * 100.0;
                
                double manualCoefficient = (lifePercent!.Value / position.MarketCapCoefficient / position.DividendCoefficient).RoundTo(2);

                var instrument = await instrumentRepository.GetInstrumentByTickerAsync(position.Ticker);
                instrument!.ManualCoefficient = manualCoefficient;
                await instrumentRepository.EditInstrumentAsync(instrument);
            }

            return new();
        }

        /// <inheritdoc />
        public async Task<GetPortfolioPositionListResponse> GetPortfolioPositionListAsync(GetPortfolioPositionListRequest request)
        {
            var storageInstruments = (await instrumentService.GetStorageInstrumentAsync())
                .Where(x => x.Type == KnownInstrumentTypes.Share || x.Type == KnownInstrumentTypes.Etf)
                .OrderBy(x => x.Ticker)
                .ToList();

            var instruments = (await instrumentRepository.GetInstrumentsAsync()) ?? [];

            var analyticInstruments = (await instrumentService.GetAnalyticInstrumentListAsync(new())).Instruments
                .Where(x => x.Type == KnownInstrumentTypes.Share || x.Type == KnownInstrumentTypes.Etf)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            var instrumentTickers = analyticInstruments.Select(x => x.Ticker).ToList();

            var analyseDataContext = await dataService.GetAnalyseDataContextAsync();

            var lifePortfolioPositions = (await lifePortfolioPositionRepository.GetLifePortfolioPositionsAsync())
                .Where(x => !x.IsDeleted)
                .Where(x => instrumentTickers.Contains(x.Ticker))
                .Where(x => x.Ticker != "TGLD")
                .Where(x => x.Ticker != "TMON")
                .ToList();

            double lifeTotalSum = 0.0;

            foreach (var position in lifePortfolioPositions)
            {
                var price = analyseDataContext.GetPrice(position.Ticker);

                if (price.HasValue)
                {
                    double positionCost = position.Size * price.Value;
                    lifeTotalSum += positionCost;
                }                                                        
            }

            await parameterRepository.SetParameterValueAsync(KnownParameters.TotalSum, lifeTotalSum.ToString("N0"));

            foreach (var instrument in analyticInstruments)
            {
                if (instrument.ManualCoefficient == 0)
                    await EditPortfolioPositionAsync(new EditPortfolioPositionRequest { Ticker = instrument.Ticker, DividendCoefficient = 1, ManualCoefficient = 1 });
            }

            analyticInstruments = (await instrumentService.GetAnalyticInstrumentListAsync(new())).Instruments
                .Where(x => x.Type == KnownInstrumentTypes.Share || x.Type == KnownInstrumentTypes.Etf)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            var tickers = analyticInstruments.Select(x => x.Ticker).ToList();
            
            var candleData = await dataService.GetCandleDataAsync(tickers);
            var dividendData = await dataService.GetDividendDataAsync(tickers);

            var keyRates = (await storageApiClient.GetKeyRateListAsync(new())).Result.KeyRates.OrderBy(x => x.Date).ToList();
            double currentKeyRate = keyRates.Last().Value ?? 0.0;

            var portfolioPositions = new List<PortfolioPositionListItem>();

            foreach (var instrument in analyticInstruments)
            {
                var portfolioPosition = new PortfolioPositionListItem()
                {
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    Sector = instruments.Find(x => x.Ticker == instrument.Ticker)?.Sector ?? string.Empty,
                    ManualCoefficient = instrument.ManualCoefficient,
                    Price = candleData[instrument.Ticker].Last().Close
                };

                double fundamentalScoreCoefficient = 10.0;

                var fundamentalScore = await fundamentalScoreService.GetFundamentalScoreAsync(instrument.Ticker);

                fundamentalScoreCoefficient = fundamentalScore?.Score.Value ?? 0.0;

                portfolioPosition.FundamentalScoreCoefficient = fundamentalScoreCoefficient.RoundTo(2);

                double dividendCoefficient = 1.0;

                if (dividendData.TryGetValue(instrument.Ticker, out Core.Models.Dividend? value))
                {                    
                    double hiLimitCoefficient = 3.0;
                    double loLimitCoefficient = 2.0;
                    double hiLimitYield = currentKeyRate;
                    double loLimitYield = hiLimitYield / 3.0 * 2.0;

                    double yield = value.Yield!.Value;

                    if (yield >= hiLimitYield) dividendCoefficient = hiLimitCoefficient;
                    else if (yield <= loLimitYield) dividendCoefficient = 1.0;
                    else dividendCoefficient = (yield - loLimitYield) * (hiLimitCoefficient - loLimitCoefficient) / (hiLimitYield - loLimitYield) + loLimitCoefficient;
                }

                portfolioPosition.DividendCoefficient = dividendCoefficient.RoundTo(2);

                portfolioPosition.TrendCoefficient = 1.0;

                if (fundamentalScore?.MarketCap?.Ratio == 0.5) portfolioPosition.MarketCapCoefficient = 1.0;
                if (fundamentalScore?.MarketCap?.Ratio == 0.75) portfolioPosition.MarketCapCoefficient = 2.0;
                if (fundamentalScore?.MarketCap?.Ratio == 1.0) portfolioPosition.MarketCapCoefficient = 3.0;
                if (instrument.Ticker == "TGLD") portfolioPosition.MarketCapCoefficient = 1.0;

                portfolioPosition.ResultCoefficient = (
                    portfolioPosition.FundamentalScoreCoefficient *
                    portfolioPosition.DividendCoefficient *
                    portfolioPosition.MarketCapCoefficient * 
                    portfolioPosition.ManualCoefficient).RoundTo(2);

                portfolioPositions.Add(portfolioPosition);
            }

            double totalSum = Convert.ToDouble(((await parameterRepository.GetParameterValueAsync(KnownParameters.TotalSum)) ?? "0").Replace(" ", "").Trim());
            
            double baseUnit = totalSum / portfolioPositions.Sum(x => x.ResultCoefficient);

            foreach (var portfolioPosition in portfolioPositions)
            {
                portfolioPosition.ResultCoefficient = Math.Round(portfolioPosition.ResultCoefficient * portfolioPosition.TrendCoefficient, 2);
                portfolioPosition.Cost = Math.Round(baseUnit * portfolioPosition.ResultCoefficient, 2);
                portfolioPosition.Percent = Math.Round(portfolioPosition.Cost / totalSum * 100.0, 2);

                if (portfolioPosition.Price.HasValue)
                {
                    int lot = storageInstruments.Find(x => x.Ticker == portfolioPosition.Ticker)?.Lot ?? 1;
                    int size = Convert.ToInt32(Math.Truncate(portfolioPosition.Cost / portfolioPosition.Price.Value));
                    portfolioPosition.Size = Convert.ToInt32(Math.Truncate(Convert.ToDouble(size) / Convert.ToDouble(lot)) * lot);
                }

                portfolioPosition.LifeSize = lifePortfolioPositions.Find(x => x.Ticker == portfolioPosition.Ticker)?.Size ?? 0;
                portfolioPosition.Delta = portfolioPosition.LifeSize - portfolioPosition.Size;
                portfolioPosition.DeltaPercent = (Convert.ToDouble(portfolioPosition.Delta) / Convert.ToDouble(portfolioPosition.Size) * 100.0).RoundTo(2);
            }

            foreach (var portfolioPosition in portfolioPositions)
                portfolioPosition.CurrentDividendYield = analyseDataContext.GetDividend(portfolioPosition.Ticker)?.Yield;

            foreach (var portfolioPosition in portfolioPositions)
            {
                double lifeCost = portfolioPositions.Where(x => x.Sector.Contains(portfolioPosition.Sector)).Select(x => x.Cost).Sum();
                double sectorPercent = (lifeCost / totalSum * 100.0).RoundTo(2);
                portfolioPosition.Sector += $" ({sectorPercent}) %";
            }

            foreach (var portfolioPosition in portfolioPositions)
            {
                var candles = analyseDataContext.GetCandles(portfolioPosition.Ticker)
                    .Where(x => x.Date >= DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)))
                    .OrderBy(x => x.Date)
                    .ToList();

                double first = candles.First().Close;
                double last = candles.Last().Close;

                portfolioPosition.MonthDeltaPricePercent = ((last - first) / first * 100.0).RoundTo(2);
            }

            var buyRecommendationPositionTickers = portfolioPositions
                .Where(x => x.DeltaPercent < 0.0)
                .Where(x => x.MonthDeltaPricePercent < 0.0)
                .Select(x => x.Ticker)
                .ToList();

            for (int i = 0; i < portfolioPositions.Count; i++)
            {
                if (buyRecommendationPositionTickers.Contains(portfolioPositions[i].Ticker))
                    portfolioPositions[i].Recommendation = "Докупить";
            }

            List<PortfolioPositionListItem> orderedPortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.Percent)];

            if (request.OrderField is not null)
            {
                if (request.OrderField == string.Empty)
                    orderedPortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.Percent)];

                else if (request.OrderField == "Percent")
                    orderedPortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.Percent)];

                else if (request.OrderField == "CurrentDividendYield")
                    orderedPortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.CurrentDividendYield)];

                else if (request.OrderField == "DeltaPercent")
                    orderedPortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.DeltaPercent)];

                else if (request.OrderField == "MonthDeltaPricePercent")
                    orderedPortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.MonthDeltaPricePercent)];
            }

            var response = new GetPortfolioPositionListResponse()
            {
                TotalSum = totalSum,
                PortfolioPositions = orderedPortfolioPositions
            };

            for (int i = 0; i < response.PortfolioPositions.Count; i++)
                response.PortfolioPositions[i].Number = i + 1;

            return response;
        }
    }
}
