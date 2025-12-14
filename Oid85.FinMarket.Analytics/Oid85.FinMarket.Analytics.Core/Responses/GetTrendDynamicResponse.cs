namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetTrendDynamicResponse
    {
        public List<DateOnly> Dates { get; set; }
        public List<TrendDynamicData> Indexes { get; set; }
        public List<TrendDynamicData> Shares { get; set; }
        public List<TrendDynamicData> Futures { get; set; }
        public List<TrendDynamicData> Bonds { get; set; }
        public List<TrendDynamicDeltaData> DeltaIndexes { get; set; }
        public List<TrendDynamicDeltaData> DeltaShares { get; set; }
        public List<TrendDynamicDeltaData> DeltaFutures { get; set; }
    }

    public class TrendDynamicData
    {
        public string Ticker { get; set; }
        public List<int> Values { get; set; }
    }

    public class TrendDynamicDeltaData
    {
        public string Ticker { get; set; }
        public List<double> Values { get; set; }
    }
}
