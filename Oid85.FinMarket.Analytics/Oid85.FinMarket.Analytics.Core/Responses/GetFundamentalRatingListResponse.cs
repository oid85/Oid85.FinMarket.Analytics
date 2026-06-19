using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetFundamentalRatingListResponse
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
        public double? FallingFromMax { get; set; } = null;
    }
}
