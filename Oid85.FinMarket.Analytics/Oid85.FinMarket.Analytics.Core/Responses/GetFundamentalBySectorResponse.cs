namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetFundamentalBySectorResponse
    {
        public string Sector { get; set; } = string.Empty;
        public List<FundamentalBySectorItem> PriceDiagram { get; set; } = [];
        public List<FundamentalBySectorItem> NetProfitDiagram { get; set; } = [];
        public List<FundamentalBySectorItem> NetDebtDiagram { get; set; } = [];
        public List<FundamentalBySectorItem> DividendDiagram { get; set; } = [];
        public List<FundamentalBySectorBubbleDiagramPoint> BubbleDiagram { get; set; } = [];
        public List<FundamentalRatingItem> FundamentalRatingItems { get; set; } = [];
    }

    public class FundamentalBySectorItem
    {
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool InPortfolio { get; set; }
        public List<FundamentalBySectorDateValue> Data { get; set; } = [];
    }

    public class FundamentalBySectorDateValue
    {
        public string? Date { get; set; } = null;
        public double? Value { get; set; } = null;
    }

    public class FundamentalBySectorBubbleDiagramPoint
    {
        public string Ticker { get; set; }
        public double? NetDebtEbitda { get; set; }
        public double? EvEbitda { get; set; }
        public double? MarketCap { get; set; }
    }
}
