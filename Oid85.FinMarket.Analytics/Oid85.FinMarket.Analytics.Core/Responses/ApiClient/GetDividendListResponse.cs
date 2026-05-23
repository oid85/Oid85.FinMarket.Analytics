namespace Oid85.FinMarket.Analytics.Core.Responses.ApiClient
{
    public class GetDividendListResponse
    {
        public GetDividendListResult Result { get; set; } = new();
    }

    public class GetDividendListResult
    {
        public List<GetDividendListItemResponse> Dividends { get; set; } = [];
    }

    public class GetDividendListItemResponse
    {
        public string Ticker { get; set; }
        public DateOnly Date { get; set; }
        public double Value { get; set; }
    }
}
