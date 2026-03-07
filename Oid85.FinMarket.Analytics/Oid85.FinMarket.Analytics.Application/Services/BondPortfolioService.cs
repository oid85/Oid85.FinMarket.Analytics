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
    public class BondPortfolioService(
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository,
        IInstrumentService instrumentService,
        IWeekTrendService weekTrendService)
        : IBondPortfolioService
    {
        /// <inheritdoc />
        public async Task<EditBondPortfolioPositionResponse> EditBondPortfolioPositionAsync(EditBondPortfolioPositionRequest request)
        {
            var instrument = await instrumentRepository.GetInstrumentByTickerAsync(request.Ticker);

            instrument!.ManualCoefficient = request.ManualCoefficient;

            await instrumentRepository.EditInstrumentAsync(instrument);

            return new();
        }

        /// <inheritdoc />
        public async Task<EditBondPortfolioTotalSumResponse> EditBondPortfolioTotalSumAsync(EditBondPortfolioTotalSumRequest request)
        {
            await parameterRepository.SetParameterValueAsync(KnownParameters.BondTotalSum, request.TotalSum.ToString("N0"));
            return new();
        }

        /// <inheritdoc />
        public async Task<GetBondPortfolioPositionListResponse> GetBondPortfolioPositionListAsync(GetBondPortfolioPositionListRequest request)
        {
            var storageInstruments = (await instrumentService.GetStorageInstrumentAsync())
                .Where(x => x.Type == KnownInstrumentTypes.Bond)
                .OrderBy(x => x.Ticker)
                .ToList();

            var instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new() { LastDaysCount = 0 })).Instruments
                .Where(x => x.Type == KnownInstrumentTypes.Bond)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            foreach (var instrument in instruments)
            {
                if (instrument.ManualCoefficient == 0)
                    await EditBondPortfolioPositionAsync(new EditBondPortfolioPositionRequest { Ticker = instrument.Ticker, ManualCoefficient = 1 });
            }

            instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new() { LastDaysCount = 0 })).Instruments
                .Where(x => x.Type == KnownInstrumentTypes.Bond)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            var portfolioPositions = new List<GetBondPortfolioPositionListItemResponse>();

            foreach (var instrument in instruments)
            {
                var storageInstrument = storageInstruments.Find(x => x.Ticker == instrument.Ticker);

                var portfolioPosition = new GetBondPortfolioPositionListItemResponse()
                {
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    ManualCoefficient = instrument.ManualCoefficient,
                    Price = storageInstrument?.LastPrice
                };

                portfolioPosition.ResultCoefficient = Math.Round(portfolioPosition.ManualCoefficient, 2);

                portfolioPositions.Add(portfolioPosition);
            }

            string parameterTotalSum = (await parameterRepository.GetParameterValueAsync(KnownParameters.BondTotalSum)) ?? "0";
            double totalSum = Convert.ToDouble(parameterTotalSum.Replace(" ", "").Trim());

            var baseUnit = totalSum / portfolioPositions.Sum(x => x.ResultCoefficient);

            foreach (var portfolioPosition in portfolioPositions)
            {
                portfolioPosition.Cost = Math.Round(baseUnit * portfolioPosition.ResultCoefficient, 2);
                portfolioPosition.Percent = Math.Round(portfolioPosition.Cost / totalSum * 100.0, 2);

                if (portfolioPosition.Price.HasValue)
                    portfolioPosition.Size = Convert.ToInt32(Math.Round(portfolioPosition.Cost / portfolioPosition.Price.Value, 0));
            }

            var response = new GetBondPortfolioPositionListResponse()
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
