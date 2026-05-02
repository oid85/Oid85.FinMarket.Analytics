namespace Oid85.FinMarket.Analytics.Core.Models
{
    public class FundamentalMetric
    {
        public string Ticker { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public double? NumberShares { get; set; } = null;
        public double? Price { get; set; } = null;
        public double? Pe { get; set; } = null;
        public double? Pbv { get; set; } = null;
        public double? Fcf { get; set; } = null;
        public double? Eps { get; set; } = null;
        public double? Roa { get; set; } = null;
        public double? Roe { get; set; } = null;
        public double? EvEbitda { get; set; } = null;
        public double? NetDebtEbitda { get; set; } = null;
        public double? Dividend { get; set; } = null;
        public double? DividendYield { get; set; } = null;
        public double? NetProfit { get; set; } = null;        
        public double? DeltaMinMax { get; set; } = null;
        public double? Ev { get; set; } = null;
        public double? Ebitda { get; set; } = null;
        public double? OwnCapital { get; set; } = null;
        public double? NetDebt { get; set; } = null;
        public double? Revenue { get; set; } = null;
        public double? EbitdaRevenue { get; set; } = null;
        public double? MarketCap { get; set; } = null;
    }
}
