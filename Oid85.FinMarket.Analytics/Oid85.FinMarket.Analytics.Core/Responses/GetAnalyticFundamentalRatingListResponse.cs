using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetAnalyticFundamentalRatingListResponse
    {
        public List<FundamentalRatingItem> Items { get; set; } = [];
    }

    public class FundamentalRatingItem
    {
        public int Number { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public bool InPortfolio { get; set; }
        public FundamentalScore? Score { get; set; } = null;
        public FundamentalMetric? Metric { get; set; } = null;
        public Forecast? Forecast { get; set; } = null;
        public Forecast? NataliaBaffetovnaForecast { get; set; } = null;
        public Forecast? FinanceMarkerForecast { get; set; } = null;
        public Forecast? VladProDengiForecast { get; set; } = null;
        public Forecast? MozgovikForecast { get; set; } = null;
        public Forecast? PredictNetProfitForecast { get; set; } = null;
    }
}
