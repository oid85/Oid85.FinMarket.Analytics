namespace Oid85.FinMarket.Analytics.Core.Requests.ApiClient
{
    public class GetLastCandleRequest
    {
        public List<string> Tickers { get; set; }
        public DateOnly Date { get; set; }
    }
}
