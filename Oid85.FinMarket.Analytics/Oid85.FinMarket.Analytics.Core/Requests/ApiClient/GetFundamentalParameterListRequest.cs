namespace Oid85.FinMarket.Analytics.Core.Requests.ApiClient
{
    public class GetFundamentalParameterListRequest
    {
        public string? Ticker { get; set; } = null;
        public List<string>? Periods { get; set; } = null;
    }
}
