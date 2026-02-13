namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetAnalyticFundamentalParameterBubbleDiagramResponse
    {
        public List<GetAnalyticFundamentalParameterBubbleDiagramPointResponse> Data { get; set; } = [];
    }

    public class GetAnalyticFundamentalParameterBubbleDiagramPointResponse
    {
        public string Ticker { get; set; }
        public double NetDebtEbitda { get; set; }
        public double EvEbitda { get; set; }
        public double MarketCap { get; set; }
    }
}
