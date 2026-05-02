namespace Oid85.FinMarket.Analytics.Core.Models
{
    public class FundamentalScore
    {
        public AnalyseParameter<double?>? Pe { get; set; } = null;
        public AnalyseParameter<double?>? Pbv { get; set; } = null;
        public AnalyseParameter<double?>? EvEbitda { get; set; } = null;
        public AnalyseParameter<double?>? NetDebtEbitda { get; set; } = null;
        public AnalyseParameter<bool?>? DividendAristocrat { get; set; } = null;
        public double ScoreValue { get; set; } = 0.0;
    }
}
