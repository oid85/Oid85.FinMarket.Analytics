namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetClosePriceDiagramResponse
    {
        public List<GetClosePriceDiagramItemResponse> Items { get; set; } = [];
    }

    public class GetClosePriceDiagramItemResponse
    {
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<GetClosePriceDiagramDateValueResponse> Data { get; set; } = [];
        public bool InPortfolio { get; set; }
        public string TrendState { get; set; } = string.Empty;
        public string? Recommendation { get; set; }
        public double? DividendYield { get; set; }
    }

    public class GetClosePriceDiagramDateValueResponse
    {
        public DateOnly? Date { get; set; } = null;
        public double? Value { get; set; } = null;
        public double? MaxTarget { get; set; } = null;
        public double? MinTarget { get; set; } = null;
        public double? ConsensusPrice { get; set; } = null;
    }
}
