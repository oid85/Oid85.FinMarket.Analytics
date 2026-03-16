using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses;
using Oid85.FinMarket.Analytics.Core.Responses.ApiClient;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class BondPortfolioService(
        IInstrumentRepository instrumentRepository,
        IParameterRepository parameterRepository,
        ILifePortfolioPositionRepository lifePortfolioPositionRepository,
        IInstrumentService instrumentService,
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient)
        : IBondPortfolioService
    {
        /// <inheritdoc />
        public async Task<EditBondPortfolioPositionResponse> EditBondPortfolioPositionAsync(EditBondPortfolioPositionRequest request)
        {
            var instrument = await instrumentRepository.GetInstrumentByTickerAsync(request.Ticker);
            instrument!.ManualCoefficient = request.ManualCoefficient;
            await instrumentRepository.EditInstrumentAsync(instrument);

            await lifePortfolioPositionRepository.EditLifePortfolioPositionAsync(request.Ticker, request.LifeSize);

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
            var lifePortfolioPositions = await lifePortfolioPositionRepository.GetLifePortfolioPositionsAsync();

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

            var couponDictionary = new Dictionary<string, List<GetBondCouponListItemResponse>>();

            foreach (var instrument in instruments)
            {
                var couponsTwoYear = (await finMarketStorageServiceApiClient.GetBondCouponListAsync(
                    new GetBondCouponListRequest
                    {
                        Ticker = instrument.Ticker,
                        From = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
                        To = DateOnly.FromDateTime(DateTime.Today.AddYears(1))
                    })).Result.BondCoupons;

                for (int i = 1; i < couponsTwoYear.Count; i++)
                    if (couponsTwoYear[i].PayOneBond == 0) couponsTwoYear[i].PayOneBond = couponsTwoYear[i - 1].PayOneBond;

                var coupons = couponsTwoYear.Where(x => x.CouponDate >= DateOnly.FromDateTime(DateTime.Today) && x.CouponDate <= DateOnly.FromDateTime(DateTime.Today.AddYears(1))).ToList();

                couponDictionary.Add(instrument.Ticker, coupons);
            }

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

                double timeCoefficient = 1.0;

                if (storageInstrument is not null && storageInstrument.MaturityDate.HasValue)
                {
                    var daysToMaturity = (storageInstrument.MaturityDate.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days;

                    if (daysToMaturity > 365 * 5) timeCoefficient = 1.4;
                    else if (daysToMaturity > 365 * 2) timeCoefficient = 1.2;
                    else timeCoefficient = 1.0;
                }

                portfolioPosition.TimeCoefficient = timeCoefficient;

                double couponCoefficient = 1.0;

                if (storageInstrument is not null && storageInstrument.LastPrice.HasValue && storageInstrument.Nkd.HasValue)
                {
                    const double loLimitCoefficient = 1.0;
                    const double hiLimitCoefficient = 2.0;
                    const double loLimitYield = 10.0;
                    const double hiLimitYield = 20.0;

                    var coupons = couponDictionary[portfolioPosition.Ticker];
                    double couponSum = coupons.Sum(x => x.PayOneBond);
                    double yield = couponSum / (storageInstrument.LastPrice.Value + storageInstrument.Nkd.Value) * 100.0;

                    if (yield >= hiLimitYield) couponCoefficient = hiLimitCoefficient;
                    else if (yield <= loLimitYield) couponCoefficient = loLimitCoefficient;
                    else couponCoefficient = (yield - loLimitYield) * (hiLimitCoefficient - loLimitCoefficient) / (hiLimitYield - loLimitYield) + loLimitCoefficient;
                }

                portfolioPosition.CouponCoefficient = Math.Round(couponCoefficient, 2);

                portfolioPosition.ResultCoefficient = Math.Round(portfolioPosition.CouponCoefficient * portfolioPosition.TimeCoefficient * portfolioPosition.ManualCoefficient, 2);

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
                    portfolioPosition.Size = Convert.ToInt32(Math.Truncate(portfolioPosition.Cost / portfolioPosition.Price.Value));

                portfolioPosition.LifeSize = lifePortfolioPositions.Find(x => x.Ticker == portfolioPosition.Ticker)?.Size ?? 0;
            }

            var response = new GetBondPortfolioPositionListResponse()
            {
                TotalSum = totalSum,
                PortfolioPositions = [.. portfolioPositions.OrderBy(x => x.Name)]
            };

            int number = 1;

            foreach (var portfolioPosition in response.PortfolioPositions)
                portfolioPosition.Number = number++;

            double yearCouponSum = 0.0;

            foreach (var portfolioPosition in response.PortfolioPositions)
            {
                var coupons = couponDictionary[portfolioPosition.Ticker];

                double couponSum = coupons.Sum(x => x.PayOneBond);
                double yearCoupon = couponSum * portfolioPosition.LifeSize;
                yearCouponSum += yearCoupon;

                portfolioPosition.YearCoupon = Math.Round(yearCoupon, 2);
            }

            response.YearCouponSum = Math.Round(yearCouponSum, 2);
            response.YearCouponPrc = Math.Round(yearCouponSum / totalSum * 100.0, 2);
            response.MonthCouponSum = Math.Round(yearCouponSum / 12.0, 2);

            response.TotalSumLongOfz = response.PortfolioPositions.Where(x => x.Name.Contains("ОФЗ")).Sum(x => x.Cost);

            return response;
        }
    }
}
