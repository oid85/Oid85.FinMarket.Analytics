namespace Oid85.FinMarket.Analytics.Core.Requests
{
    public class DeleteAnalyticFundamentalParameterRequest
    {
        public string Ticker { get; set; }
        public string Type { get; set; }
        public string Period { get; set; }
    }
}
