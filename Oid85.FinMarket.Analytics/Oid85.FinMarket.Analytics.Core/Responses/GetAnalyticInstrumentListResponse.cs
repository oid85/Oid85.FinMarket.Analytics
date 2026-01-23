namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetAnalyticInstrumentListResponse
    {
        public List<GetAnalyticInstrumentListItemResponse> Instruments { get; set; }
    }

    public class GetAnalyticInstrumentListItemResponse
    {
        public Guid Id { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }        
        public bool IsSelected { get; set; }
        public string Type { get; set; }
        public double BenchmarkChange { get; set; }
    }
}
