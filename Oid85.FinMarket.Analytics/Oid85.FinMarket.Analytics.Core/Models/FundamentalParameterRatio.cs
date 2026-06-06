using Oid85.FinMarket.Analytics.Common.KnownConstants;

namespace Oid85.FinMarket.Analytics.Core.Models
{
    public class FundamentalParameterRatio
    {
        public double Ratio { get; set; } = 0.0;
        public string Color { get; set; } = KnownColors.White;
        public string? Description { get; set; } = null;
        public string? Text { get; set; } = null;
    }
}
