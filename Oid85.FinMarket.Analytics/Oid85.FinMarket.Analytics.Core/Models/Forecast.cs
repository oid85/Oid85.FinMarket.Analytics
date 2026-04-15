namespace Oid85.FinMarket.Analytics.Core.Models
{
    public class Forecast
    {
        public string Ticker { get; set; }
        public double? CurrentPrice { get; set; }
        public double? ConsensusPrice { get; set; }
        public double? UpsidePrc { get; set; }
        public double? MinTarget { get; set; }
        public double? MaxTarget { get; set; }
        public string? Recommendation { get; set; }
    }
}
