namespace Oid85.FinMarket.Analytics.Core.Requests
{
    public class EditPortfolioPositionRequest
    {
        public string Ticker { get; set; }
        public double DividendCoefficient { get; set; }
        public double ManualCoefficient { get; set; }
    }
}
