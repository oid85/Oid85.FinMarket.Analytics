namespace Oid85.FinMarket.Analytics.Core.Requests.ApiClient
{
    public class GetInstrumentPriceRequest
    {
        public string AccountName { get; set; } = string.Empty;
        public List<string> Tickers { get; set; } = [];
    }
}
