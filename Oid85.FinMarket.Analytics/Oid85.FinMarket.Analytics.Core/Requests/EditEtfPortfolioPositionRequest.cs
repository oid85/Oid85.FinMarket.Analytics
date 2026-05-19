namespace Oid85.FinMarket.Analytics.Core.Requests
{
    public class EditEtfPortfolioPositionRequest
    {
        public string Ticker { get; set; }
        public double ManualCoefficient { get; set; }
        public int LifeSize { get; set; }
    }
}
