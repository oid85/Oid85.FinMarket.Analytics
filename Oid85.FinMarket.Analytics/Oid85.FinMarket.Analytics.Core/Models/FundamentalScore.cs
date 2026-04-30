namespace Oid85.FinMarket.Analytics.Core.Models
{
    public class FundamentalScore
    {
        public Parameter<double>? Pe { get; set; } = null;
        public bool PeOk { get; set; } = false;
        public bool EvOk { get; set; } = false;
        public bool PbvOk { get; set; } = false;
        public bool DividendYieldOk { get; set; } = false;
        public bool IsDividendAristocrat { get; set; } = false;
        public bool NetProfitOk { get; set; } = false;
        public bool EpsOk { get; set; } = false;
        public bool FcfOk { get; set; } = false;
        public bool NetDebtOk{ get; set; } = false;
        public double ScoreValue { get; set; } = 0.0;
    }
}
