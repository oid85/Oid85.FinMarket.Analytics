namespace Oid85.FinMarket.Analytics.Core.Requests
{
    public class CreateOrUpdateAnalyticFundamentalParameterRequest
    {
        public string Ticker { get; set; }
        public string Type { get; set; }
        public string Period { get; set; }
        public string Value { get; set; }
    }
}
