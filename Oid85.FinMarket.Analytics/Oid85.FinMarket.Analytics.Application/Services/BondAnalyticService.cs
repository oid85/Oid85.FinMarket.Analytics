using Oid85.FinMarket.Analytics.Application.Interfaces.ApiClients;
using Oid85.FinMarket.Analytics.Application.Interfaces.Repositories;
using Oid85.FinMarket.Analytics.Application.Interfaces.Services;
using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Requests;
using Oid85.FinMarket.Analytics.Core.Requests.ApiClient;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Services
{
    /// <inheritdoc />
    public class BondAnalyticService(
        IInstrumentRepository instrumentRepository,
        IInstrumentService instrumentService,
        IFinMarketStorageServiceApiClient finMarketStorageServiceApiClient) 
        : IBondAnalyticService
    {
        /// <inheritdoc />
        public async Task<GetBondAnalyticResponse> GetBondAnalyticAsync(GetBondAnalyticRequest request)
        {
            var instruments = (await instrumentService.GetStorageInstrumentAsync() ?? []).Where(x => x.Type == KnownInstrumentTypes.Bond).OrderBy(x => x.Ticker).ToList();
            var from = DateOnly.FromDateTime(DateTime.Today);
            var to = DateOnly.FromDateTime(DateTime.Today.AddYears(1));
            var dates = DateUtils.GetMonthDates(from, to);

            var response = new GetBondAnalyticResponse();

            var bondAnalyticItems = new List<GetBondAnalyticItemResponse>();

            foreach (var instrument in instruments)
            {
                var bondAnalyticItem = new GetBondAnalyticItemResponse
                {
                    Ticker = instrument.Ticker,
                    Name = instrument.Name,
                    Price = instrument.LastPrice ?? 0.0,
                    Nkd = instrument.Nkd ?? 0.0
                };

                var coupons = (await finMarketStorageServiceApiClient.GetBondCouponListAsync(new GetBondCouponListRequest { Ticker = instrument.Ticker, From = from, To = to })).Result.BondCoupons;

                foreach (var date in dates)
                {
                    var coupon = coupons.Find(x => x.CouponDate.Month == date.Month && x.CouponDate.Year == date.Year);

                    bondAnalyticItem.Coupons.Add(new GetBondAnalyticCouponData 
                    { 
                        Date = date, 
                        CouponSum = coupon?.PayOneBond
                    });
                }

                var couponTotalSum = coupons.Sum(x => x.PayOneBond);

                if (instrument.LastPrice.HasValue && instrument.Nkd.HasValue)
                    bondAnalyticItem.Yield = Math.Round(couponTotalSum / (instrument.LastPrice.Value + instrument.Nkd.Value) * 100.0, 2);

                bondAnalyticItems.Add(bondAnalyticItem);
            }            

            response.Items = [.. bondAnalyticItems.OrderByDescending(x => x.Yield)];

            return response;
        }
    }
}
