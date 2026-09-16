namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetFundamentalRatingShortListResponse
    {
        public List<FundamentalRatingShortListItem> Items { get; set; } = [];
    }

    public class FundamentalRatingShortListItem
    {
        public string Ticker { get; set; } = string.Empty;
        public double Score { get; set; }
        public double DividendYieldRatio { get; set; }
        public double DividendAristocratRatio { get; set; }
    }
}
