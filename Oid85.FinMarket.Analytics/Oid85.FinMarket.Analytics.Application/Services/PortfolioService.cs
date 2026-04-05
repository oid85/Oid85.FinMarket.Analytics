using Oid85.FinMarket.Analytics.Application.Helpers;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Core.Enums;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class PortfolioService(
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository,
        ILifePortfolioPositionRepository lifePortfolioPositionRepository,
        IInstrumentService instrumentService,
        IDataService dataService)
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
                await lifePortfolioPositionRepository.EditLifePortfolioPositionAsync(request.Ticker, request.LifeSize);

            return new();
        }

        /// <inheritdoc />
        public async Task<EditPortfolioTotalSumResponse> EditPortfolioTotalSumAsync(EditPortfolioTotalSumRequest request)
        {
            await parameterRepository.SetParameterValueAsync(KnownParameters.TotalSum, request.TotalSum.ToString("N0"));
            return new();
        }

        /// <inheritdoc />
        public async Task<GetPortfolioPositionListResponse> GetPortfolioPositionListAsync(GetPortfolioPositionListRequest request)
        {
            var lifePortfolioPositions = await lifePortfolioPositionRepository.GetLifePortfolioPositionsAsync();

            var storageInstruments = (await instrumentService.GetStorageInstrumentAsync())
                .Where(x => x.Type == KnownInstrumentTypes.Share).OrderBy(x => x.Ticker).ToList();

            var instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new())).Instruments
                .Where(x => x.Type == KnownInstrumentTypes.Share)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            foreach (var instrument in instruments)
            {
                if (instrument.ManualCoefficient == 0)
                    await EditPortfolioPositionAsync(new EditPortfolioPositionRequest { Ticker = instrument.Ticker, DividendCoefficient = 1, ManualCoefficient = 1 });
            }

            instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new())).Instruments
                .Where(x => x.Type == KnownInstrumentTypes.Share)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            var tickers = instruments.Select(x => x.Ticker).ToList();

            var ultimateSmootherData = await dataService.GetUltimateSmootherDataAsync(tickers);
            var candleData = await dataService.GetCandleDataAsync(tickers);
            var dividendData = await dataService.GetDividendDataAsync(tickers);

            var portfolioPositions = new List<GetPortfolioPositionListItemResponse>();

            foreach (var instrument in instruments)
            {
                var portfolioPosition = new GetPortfolioPositionListItemResponse()
                {
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    ManualCoefficient = instrument.ManualCoefficient,
                    Price = candleData[instrument.Ticker].Last().Close
                };

                double dividendCoefficient = 1.0;

                if (dividendData.ContainsKey(instrument.Ticker))
                {
                    const double loLimitCoefficient = 1.0;
                    const double hiLimitCoefficient = 2.0;
                    const double loLimitYield = 10.0;
                    const double hiLimitYield = 20.0;

                    double yield = dividendData[instrument.Ticker].Yield!.Value;

                    if (yield >= hiLimitYield) dividendCoefficient = hiLimitCoefficient;
                    else if (yield <= loLimitYield) dividendCoefficient = loLimitCoefficient;
                    else dividendCoefficient = (yield - loLimitYield) * (hiLimitCoefficient - loLimitCoefficient) / (hiLimitYield - loLimitYield) + loLimitCoefficient;
                }

                portfolioPosition.DividendCoefficient = Math.Round(dividendCoefficient, 2);

                var trendState = TrendStateHelper.GetTrendState(ultimateSmootherData[instrument.Ticker]);

                switch (trendState.TrendState)
                {
                    case TrendState.UpTrend:
                        portfolioPosition.TrendCoefficient = 1.0;
                        portfolioPosition.Message = trendState.Message;
                        break;

                    case TrendState.NoTrend:
                        portfolioPosition.TrendCoefficient = 0.7;
                        portfolioPosition.Message = trendState.Message;
                        break;

                    case TrendState.DownTrend:
                        portfolioPosition.TrendCoefficient = dividendCoefficient > 1.0 ? 0.7 : 0.0;
                        portfolioPosition.Message = trendState.Message;
                        break;
                }
                
                portfolioPosition.ManualCoefficient = instrument.ManualCoefficient;

                portfolioPosition.ResultCoefficient = Math.Round(portfolioPosition.DividendCoefficient * portfolioPosition.ManualCoefficient, 2);

                portfolioPositions.Add(portfolioPosition);
            }
            
            double totalSum = Convert.ToDouble(((await parameterRepository.GetParameterValueAsync(KnownParameters.TotalSum)) ?? "0").Replace(" ", "").Trim());
            
            int minTotalNumberSharesInPortfolio = Convert.ToInt32((await parameterRepository.GetParameterValueAsync(KnownParameters.MinTotalNumberSharesInPortfolio)) ?? "0");

            double baseUnit = portfolioPositions.Count < minTotalNumberSharesInPortfolio
                ? totalSum / (portfolioPositions.Sum(x => x.ResultCoefficient) + (minTotalNumberSharesInPortfolio - portfolioPositions.Count))
                : totalSum / portfolioPositions.Sum(x => x.ResultCoefficient);

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
            }

            var response = new GetPortfolioPositionListResponse()
            {
                TotalSum = totalSum,
                PortfolioPositions = [.. portfolioPositions.OrderByDescending(x => x.Percent)]
            };

            int number = 1;

            foreach (var portfolioPosition in response.PortfolioPositions)
                portfolioPosition.Number = number++;

            return response;
        }
    }
}
