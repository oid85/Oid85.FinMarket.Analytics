namespace Oid85.FinMarket.Analytics.Core.Responses.ApiClient
{
    public class GetInstrumentPriceResponse
    {
        public GetInstrumentPriceResult Result { get; set; } = new();
    }

    public class GetInstrumentPriceResult
    {
        public List<TickerPriceItem> Items { get; set; } = [];
    }

    public class TickerPriceItem
    {
        public string Ticker { get; set; } = string.Empty;
        public double Price { get; set; }
    }
}
