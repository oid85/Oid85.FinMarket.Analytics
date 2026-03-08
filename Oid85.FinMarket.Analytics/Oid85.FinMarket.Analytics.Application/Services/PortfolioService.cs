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
        IInstrumentService instrumentService,
        IWeekTrendService weekTrendService)
        : IPortfolioService
    {
        /// <inheritdoc />
        public async Task<EditPortfolioPositionResponse> EditPortfolioPositionAsync(EditPortfolioPositionRequest request)
        {
            var instrument = await instrumentRepository.GetInstrumentByTickerAsync(request.Ticker);

            instrument!.DividendCoefficient = request.DividendCoefficient;
            instrument.ManualCoefficient = request.ManualCoefficient;

            await instrumentRepository.EditInstrumentAsync(instrument);

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
            var weekDeltaDataItems = (await weekTrendService.GetWeekDeltaAsync(new GetWeekDeltaRequest { LastWeeksCount = 5 })).Shares;

            var instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new() { LastDaysCount = 90 })).Instruments
                .Where(x => x.Type == KnownInstrumentTypes.Share)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            foreach (var instrument in instruments)
            {
                if (instrument.DividendCoefficient == 0 || instrument.ManualCoefficient == 0)
                    await EditPortfolioPositionAsync(new EditPortfolioPositionRequest { Ticker = instrument.Ticker, DividendCoefficient = 1, ManualCoefficient = 1 });
            }

            instruments = (await instrumentService.GetAnalyticInstrumentListAsync(new() { LastDaysCount = 90 })).Instruments
                .Where(x => x.Type == KnownInstrumentTypes.Share)
                .Where(x => x.InPortfolio)
                .OrderBy(x => x.Ticker)
                .ToList();

            var portfolioPositions = new List<GetPortfolioPositionListItemResponse>();

            foreach (var instrument in instruments)
            {
                var weekDeltaData = weekDeltaDataItems.Find(x => x.Ticker == instrument.Ticker);

                var portfolioPosition = new GetPortfolioPositionListItemResponse()
                {
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    DividendCoefficient = instrument.DividendCoefficient,
                    ManualCoefficient = instrument.ManualCoefficient,
                    Price = weekDeltaData!.Items.Last().Price
                };

                var trendState = TrendStateHelper.GetTrendState(weekDeltaData.Items);

                switch (trendState.TrendState)
                {
                    case TrendState.Trend:
                        portfolioPosition.TrendCoefficient = 1.3;
                        portfolioPosition.DividendCoefficient = instrument.DividendCoefficient;
                        portfolioPosition.ManualCoefficient = instrument.ManualCoefficient;
                        portfolioPosition.Message = trendState.Message;
                        break;

                    case TrendState.StrongTrend:
                        portfolioPosition.TrendCoefficient = 1.6;
                        portfolioPosition.DividendCoefficient = instrument.DividendCoefficient;
                        portfolioPosition.ManualCoefficient = instrument.ManualCoefficient;
                        portfolioPosition.Message = trendState.Message;
                        break;

                    case TrendState.BreakTrend:
                        portfolioPosition.TrendCoefficient = 0.5;
                        portfolioPosition.DividendCoefficient = 1.0;
                        portfolioPosition.ManualCoefficient = 1.0;
                        portfolioPosition.Message = trendState.Message;
                        break;

                    case TrendState.NoTrend:
                    case TrendState.Unknown:
                    default:
                        portfolioPosition.TrendCoefficient = 1.0;
                        portfolioPosition.DividendCoefficient = instrument.DividendCoefficient;
                        portfolioPosition.ManualCoefficient = instrument.ManualCoefficient;
                        portfolioPosition.Message = trendState.Message;
                        break;
                }

                portfolioPosition.ResultCoefficient = Math.Round(portfolioPosition.DividendCoefficient * portfolioPosition.ManualCoefficient * portfolioPosition.TrendCoefficient, 2);

                portfolioPositions.Add(portfolioPosition);
            }

            string parameterTotalSum = (await parameterRepository.GetParameterValueAsync(KnownParameters.TotalSum)) ?? "0";
            double totalSum = Convert.ToDouble(parameterTotalSum.Replace(" ", "").Trim());

            var baseUnit = totalSum / portfolioPositions.Sum(x => x.ResultCoefficient);

            foreach (var portfolioPosition in portfolioPositions)
            {
                portfolioPosition.Cost = Math.Round(baseUnit * portfolioPosition.ResultCoefficient, 2);
                portfolioPosition.Percent = Math.Round(portfolioPosition.Cost / totalSum * 100.0, 2);

                if (portfolioPosition.Price.HasValue)
                    portfolioPosition.Size = Convert.ToInt32(Math.Truncate(portfolioPosition.Cost / portfolioPosition.Price.Value));
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
