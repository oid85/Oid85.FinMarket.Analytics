namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetTrendAggregateDiagramResponse
    {
        public List<GetTrendAggregateDiagramDateValueResponse> Data { get; set; } = [];
    }

    public class GetTrendAggregateDiagramDateValueResponse
    {
        public DateOnly? Date { get; set; } = null;
        public double? Value { get; set; } = null;
    }
}
