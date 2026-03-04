using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class BondAnalyseService(
        IInstrumentService instrumentService,
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient) 
        : IBondAnalyseService
    {
        /// <inheritdoc />
        public async Task<GetBondAnalyseResponse> GetBondAnalyseAsync(GetBondAnalyseRequest request)
        {
            var instruments = (await instrumentService.GetStorageInstrumentAsync() ?? [])
                .Where(x => x.Type == KnownInstrumentTypes.Bond)
                .Where(x => x.LastPrice is not null)
                .Where(x => x.Nominal is not null)
                .Where(x => x.Currency is not null)
                .Where(x => x.LastPrice > 0)
                .Where(x => x.Nominal == 1000)
                .Where(x => string.Equals(x.Currency, KnownCurrencies.Rub, StringComparison.InvariantCultureIgnoreCase))
                .OrderBy(x => x.Ticker)
                .ToList();

            var from = DateOnly.FromDateTime(DateTime.Today);
            var to = DateOnly.FromDateTime(DateTime.Today.AddYears(1));

            var dates = DateUtils.GetMonthDates(from, to);

            var response = new GetBondAnalyseResponse() { Dates = dates };

            var bondAnalyseItems = new List<GetBondAnalyseItemResponse>();

            foreach (var instrument in instruments)
            {
                var bondAnalyseItem = new GetBondAnalyseItemResponse
                {
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    Price = instrument.LastPrice.HasValue ? Math.Round(instrument.LastPrice.Value, 2) : 0.0,
                    Nkd = instrument.Nkd.HasValue ? Math.Round(instrument.Nkd.Value, 2) : 0.0
                };

                if (instrument.MaturityDate.HasValue)
                    bondAnalyseItem.DaysToMaturity = (instrument.MaturityDate.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days;

                var couponsTwoYear = (await finMarketStorageServiceApiClient.GetBondCouponListAsync(
                    new GetBondCouponListRequest 
                    { 
                        Ticker = instrument.Ticker, 
                        From = DateOnly.FromDateTime(DateTime.Today.AddYears(-1)), 
                        To = DateOnly.FromDateTime(DateTime.Today.AddYears(1))
                    })).Result.BondCoupons;

                for (int i = 1; i < couponsTwoYear.Count; i++)
                    if (couponsTwoYear[i].PayOneBond == 0) couponsTwoYear[i].PayOneBond = couponsTwoYear[i - 1].PayOneBond;

                var coupons = couponsTwoYear.Where(x => x.CouponDate > from && x.CouponDate <= to).ToList();

                foreach (var date in dates)
                {
                    var coupon = coupons.Find(x => x.CouponDate.Month == date.Month && x.CouponDate.Year == date.Year);

                    bondAnalyseItem.Coupons.Add(new GetBondAnalyseCouponData 
                    { 
                        Date = date, 
                        CouponSum = coupon?.PayOneBond
                    });
                }

                var couponTotalSum = coupons.Sum(x => x.PayOneBond);

                if (instrument.LastPrice.HasValue && instrument.Nkd.HasValue)
                    bondAnalyseItem.Yield = Math.Round(couponTotalSum / (instrument.LastPrice.Value + instrument.Nkd.Value) * 100.0, 2);

                bondAnalyseItems.Add(bondAnalyseItem);
            }            

            response.Items = [.. bondAnalyseItems.OrderByDescending(x => x.Yield)];

            return response;
        }
    }
}
