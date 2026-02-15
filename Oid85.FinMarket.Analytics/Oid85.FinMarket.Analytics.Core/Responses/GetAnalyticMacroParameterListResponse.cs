namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetAnalyticMacroParameterListResponse
    {
        public List<GetAnalyticMacroParameterItemListResponse> MacroParameters { get; set; }
    }

    public class GetAnalyticMacroParameterItemListResponse
    {
        public DateOnly Date { get; set; }
        public double? MoexIndex { get; set; }
        public double? MoexIndexChange { get; set; }
        public double? M0 { get; set; }
        public double? M0Change { get; set; }
        public double? M1 { get; set; }
        public double? M1Change { get; set; }
        public double? M2 { get; set; }
        public double? M2Change { get; set; }
        public double? M2X { get; set; }
        public double? M2XChange { get; set; }
        public double? Currency { get; set; }
        public double? CurrencyChange { get; set; }
        public double? Deposits { get; set; }
        public double? DepositsChange { get; set; }
        public double? ConsumerPriceIndexChange { get; set; }
        public double? M1ConsumerPriceIndexDifferenceChange { get; set; }
        public double? KeyRate { get; set; }
    }
}
