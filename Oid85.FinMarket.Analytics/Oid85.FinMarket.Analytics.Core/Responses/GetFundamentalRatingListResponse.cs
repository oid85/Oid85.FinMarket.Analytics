using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetFundamentalRatingListResponse
    {
        public List<FundamentalRating> Items { get; set; } = [];
        public string TickerList { get; set; } = string.Empty;
    }
}
