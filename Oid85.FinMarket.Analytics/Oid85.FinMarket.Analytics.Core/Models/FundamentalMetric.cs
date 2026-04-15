namespace Oid85.FinMarket.Analytics.Core.Models
{
    public class FundamentalMetric
    {
        public string Period { get; set; }
        public double? Pe { get; set; } = null;
        public double? Pbv { get; set; } = null;
        public double? Roa { get; set; } = null;
        public double? EvEbitda { get; set; } = null;
        public double? NetDebtEbitda { get; set; } = null;
        public double? Dividend { get; set; } = null;
        public double? NetProfit { get; set; } = null;
    }
}
