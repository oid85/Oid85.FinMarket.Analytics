namespace Oid85.FinMarket.Analytics.Core.Models
{
    public class FundamentalRating
    {
        public int Number { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public bool InPortfolio { get; set; }
        public FundamentalScore? Score { get; set; } = null;
        public FundamentalMetric? Metric { get; set; } = null;
        public double? FallingFromMax { get; set; } = null;
    }
}
