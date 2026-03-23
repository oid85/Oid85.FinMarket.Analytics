namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetFundamentalBySectorResponse
    {
        public List<GetFundamentalBySectorItemResponse> PriceDiagram { get; set; } = [];
        public List<GetFundamentalBySectorItemResponse> RevenueDiagram { get; set; } = [];
        public List<GetFundamentalBySectorItemResponse> NetProfitDiagram { get; set; } = [];
        public List<GetFundamentalBySectorItemResponse> DividendDiagram { get; set; } = [];
        public List<GetFundamentalBySectorBubbleDiagramPointResponse> BubbleDiagram { get; set; } = [];
    }

    public class GetFundamentalBySectorItemResponse
    {
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool InPortfolio { get; set; }
        public List<GetFundamentalBySectorDateValueResponse> Data { get; set; } = [];        
    }

    public class GetFundamentalBySectorDateValueResponse
    {
        public string? Date { get; set; } = null;
        public double? Value { get; set; } = null;
    }

    public class GetFundamentalBySectorBubbleDiagramPointResponse
    {
        public string Ticker { get; set; }
        public double NetDebtEbitda { get; set; }
        public double EvEbitda { get; set; }
        public double MarketCap { get; set; }
    }
}
