namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetFundamentalBySectorResponse
    {
        public List<GetFundamentalBySectorItemResponse> PriceDiagram { get; set; } = [];
        public List<GetFundamentalBySectorItemResponse> RevenueDiagram { get; set; } = [];
        public List<GetFundamentalBySectorItemResponse> NetProfitDiagram { get; set; } = [];
        public List<GetFundamentalBySectorItemResponse> DividendDiagram { get; set; } = [];
    }

    public class GetFundamentalBySectorItemResponse
    {
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<GetFundamentalBySectorDateValueResponse> Data { get; set; } = [];
    }

    public class GetFundamentalBySectorDateValueResponse
    {
        public DateOnly? Date { get; set; } = null;
        public double? Value { get; set; } = null;
    }
}
