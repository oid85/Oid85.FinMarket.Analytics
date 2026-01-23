namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetCompareTrendResponse
    {
        public List<GetCompareTrendSeriesResponse> Series { get; set; } = [];
    }

    public class GetCompareTrendSeriesResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public List<GetCompareTrendSeriesItemResponse> Data { get; set; } = [];
    }

    public class GetCompareTrendSeriesItemResponse
    {
        public DateOnly Date { get; set; }
        public double? Value { get; set; } = null;
    }
}
