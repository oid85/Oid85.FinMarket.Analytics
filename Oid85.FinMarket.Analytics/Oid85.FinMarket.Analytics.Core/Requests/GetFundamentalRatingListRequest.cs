namespace Oid85.FinMarket.Analytics.Core.Requests
{
    public class GetFundamentalRatingListRequest
    {
        public string? Sector { get; set; } = null;
        public string? FilterType { get; set; } = null;
    }
}
