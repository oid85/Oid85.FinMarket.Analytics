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
            var instruments = (await instrumentService.GetStorageInstrumentAsync() ?? []).Where(x => x.Type == KnownInstrumentTypes.Bond).OrderBy(x => x.Ticker).ToList();
            var from = DateOnly.FromDateTime(DateTime.Today);
            var to = DateOnly.FromDateTime(DateTime.Today.AddYears(1));
            var dates = DateUtils.GetMonthDates(from, to);

            var response = new GetBondAnalyseResponse();

            var bondAnalyseItems = new List<GetBondAnalyseItemResponse>();

            foreach (var instrument in instruments)
            {
                var bondAnalyseItem = new GetBondAnalyseItemResponse
                {
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    Price = instrument.LastPrice ?? 0.0,
                    Nkd = instrument.Nkd ?? 0.0
                };

                if (instrument.MaturityDate.HasValue)
                    bondAnalyseItem.DaysToMaturity = (instrument.MaturityDate.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days;

                var coupons = (await finMarketStorageServiceApiClient.GetBondCouponListAsync(new GetBondCouponListRequest { Ticker = instrument.Ticker, From = from, To = to })).Result.BondCoupons;

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
