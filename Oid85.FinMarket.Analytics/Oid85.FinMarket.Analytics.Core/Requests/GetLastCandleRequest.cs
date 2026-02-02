namespace Oid85.FinMarket.Analytics.Core.Requests
{
    public class GetLastCandleRequest
    {
        public List<string> Tickers { get; set; }
        public DateOnly Date { get; set; }
    }
}
