namespace Oid85.FinMarket.Analytics.Core.Requests.ApiClient
{
    public class GetBondCouponListRequest
    {
        public string Ticker { get; set; }
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
    }
}
