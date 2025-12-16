namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetTrendDynamicResponse
    {
        public List<DateOnly> Dates { get; set; }
        public List<TrendDynamicData> Indexes { get; set; }
        public List<TrendDynamicData> Shares { get; set; }
        public List<TrendDynamicData> Futures { get; set; }
        public List<TrendDynamicData> Bonds { get; set; }
    }

    public class TrendDynamicData
    {
        public string Ticker { get; set; }
        public List<TrendDynamicDataItem> Items { get; set; }
    }

    public class TrendDynamicDataItem
    {
        public DateOnly Date { get; set; }
        public int? Trend { get; set; }
        public double? Delta { get; set; }
    }
}
