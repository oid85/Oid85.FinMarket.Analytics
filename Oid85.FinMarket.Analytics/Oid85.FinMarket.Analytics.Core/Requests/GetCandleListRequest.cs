namespace Oid85.FinMarket.Analytics.Core.Requests
{
    public class GetCandleListRequest
    {
        public string Ticker { get; set; }
        public DateOnly From { get; set; }
        public DateOnly To { get; set; }
    }
}
