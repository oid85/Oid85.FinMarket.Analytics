namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetAnalyticMacroParameterListResponse
    {
        public List<GetAnalyticMacroParameterItemListResponse> MacroParameters { get; set; }
    }

    public class GetAnalyticMacroParameterItemListResponse
    {
        public DateOnly Date { get; set; }
        public double? IMOEX { get; set; }
        public double? M0 { get; set; }
        public double? M1 { get; set; }
        public double? M2 { get; set; }
        public double? M2X { get; set; }
        public double? ConsumerPriceIndexChange { get; set; }
        public double? KeyRate { get; set; }
    }
}
