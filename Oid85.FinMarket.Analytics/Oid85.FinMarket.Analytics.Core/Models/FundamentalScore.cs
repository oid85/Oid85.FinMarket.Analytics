namespace Oid85.FinMarket.Analytics.Core.Models
{
    public class FundamentalScore
    {
        public AnalyseRatioParameter<double?>? MarketCap { get; set; } = null;
        public AnalyseRatioParameter<double?>? Pe { get; set; } = null;
        public AnalyseRatioParameter<double?>? Pbv { get; set; } = null;
        public AnalyseRatioParameter<double?>? EvEbitda { get; set; } = null;
        public AnalyseRatioParameter<double?>? NetDebtEbitda { get; set; } = null;
        public AnalyseRatioParameter<double?>? DebtRatio { get; set; } = null;
        public AnalyseRatioParameter<double?>? DebtEquity { get; set; } = null;
        public AnalyseRatioParameter<double?>? NetProfit { get; set; } = null;
        public AnalyseRatioParameter<double?>? Fcf { get; set; } = null;
        public AnalyseRatioParameter<double?>? Eps { get; set; } = null;
        public AnalyseRatioParameter<double?>? Roa { get; set; } = null;
        public AnalyseRatioParameter<double?>? Roe { get; set; } = null;
        public AnalyseRatioParameter<double?>? EbitdaRevenue { get; set; } = null;
        public AnalyseRatioParameter<double?>? DividendYield { get; set; } = null;
        public AnalyseRatioParameter<bool?>? DividendAristocrat { get; set; } = null;
        public AnalyseParameter<double> Score { get; set; } = new();
    }
}
