namespace Oid85.FinMarket.Analytics.Core.Requests
{
    public class CreateOrUpdateAnalyticMacroParameterRequest
    {
        public DateOnly Date { get; set; }
        public double? M0 { get; set; }
        public double? M1 { get; set; }
        public double? M2 { get; set; }
        public double? M2X { get; set; }
        public double? ConsumerPriceIndexChange { get; set; }
        public double? KeyRate { get; set; }
    }
}
