using Oid85.FinMarket.Analytics.Application.Helpers;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Enums;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class EtfPortfolioService(
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository,
        ILifePortfolioPositionRepository lifePortfolioPositionRepository,
        IInstrumentService instrumentService,
        IDataService dataService)
        : IEtfPortfolioService
    {
        /// <inheritdoc />
        public async Task<EditEtfPortfolioPositionResponse> EditPortfolioPositionAsync(EditEtfPortfolioPositionRequest request)
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
        public async Task<GetEtfPortfolioPositionListResponse> GetPortfolioPositionListAsync(GetEtfPortfolioPositionListRequest request)
        {
            var storageInstruments = (await instrumentService.GetStorageInstrumentAsync())
                .Where(x => x.Type == KnownInstrumentTypes.Etf).OrderBy(x => x.Ticker).ToList();

            var instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new())).Instruments
                .Where(x => x.Type == KnownInstrumentTypes.Etf)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            var etfTickers = instruments.Select(x => x.Ticker).ToList();            

            var lifePortfolioPositions = (await lifePortfolioPositionRepository.GetLifePortfolioPositionsAsync())
                .Where(x => !x.IsDeleted)
                .Where(x => etfTickers.Contains(x.Ticker))
                .ToList();

            double lifeTotalSum = 0.0;

            foreach (var position in lifePortfolioPositions)
            {
                var price = storageInstruments.Find(x => x.Ticker == position.Ticker)?.LastPrice;

                if (price.HasValue)
                    lifeTotalSum += position.Size * price.Value;
            }

            await parameterRepository.SetParameterValueAsync(KnownParameters.EtfTotalSum, lifeTotalSum.ToString("N0"));

            foreach (var instrument in instruments)
            {
                if (instrument.ManualCoefficient == 0)
                    await EditPortfolioPositionAsync(new EditEtfPortfolioPositionRequest { Ticker = instrument.Ticker, ManualCoefficient = 1 });
            }

            instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new())).Instruments
                .Where(x => x.Type == KnownInstrumentTypes.Etf)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            var tickers = instruments.Select(x => x.Ticker).ToList();

            var portfolioPositions = new List<EtfPortfolioPositionListItem>();

            foreach (var instrument in instruments)
            {
                var portfolioPosition = new EtfPortfolioPositionListItem()
                {
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,                    
                    ManualCoefficient = instrument.ManualCoefficient,
                    Price = storageInstruments.Find(x => x.Ticker == instrument.Ticker)?.LastPrice
                };

                portfolioPosition.ManualCoefficient = instrument.ManualCoefficient;

                portfolioPosition.ResultCoefficient = Math.Round(portfolioPosition.ManualCoefficient, 2);

                portfolioPositions.Add(portfolioPosition);
            }

            double totalSum = Convert.ToDouble(((await parameterRepository.GetParameterValueAsync(KnownParameters.EtfTotalSum)) ?? "0").Replace(" ", "").Trim());

            double baseUnit = totalSum / portfolioPositions.Sum(x => x.ResultCoefficient);

            foreach (var portfolioPosition in portfolioPositions)
            {
                portfolioPosition.ResultCoefficient = Math.Round(portfolioPosition.ResultCoefficient, 2);
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

            var response = new GetEtfPortfolioPositionListResponse()
            {
                TotalSum = totalSum,
                PortfolioPositions = [.. portfolioPositions.OrderBy(x => x.Ticker)]
            };

            int number = 1;

            foreach (var portfolioPosition in response.PortfolioPositions)
                portfolioPosition.Number = number++;

            return response;
        }
    }
}
