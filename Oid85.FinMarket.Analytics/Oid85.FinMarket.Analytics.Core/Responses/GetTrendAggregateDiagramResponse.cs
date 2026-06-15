namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetTrendAggregateDiagramResponse
    {
        public List<TrendAggregateSeries> Series { get; set; } = [];
    }

    public class TrendAggregateSeries
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string ColorFill { get; set; } = string.Empty;
        public List<TrendAggregateSeriesItem> Data { get; set; } = [];
    }

    public class TrendAggregateSeriesItem
    {
        public DateOnly Date { get; set; }
        public double? Value { get; set; } = null;
    }	
}
