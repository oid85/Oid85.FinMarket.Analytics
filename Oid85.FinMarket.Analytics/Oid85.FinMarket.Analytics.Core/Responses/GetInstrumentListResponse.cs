namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetInstrumentListResponse
    {
        public GetInstrumentListResult Result { get; set; }
    }

    public class GetInstrumentListResult
    {
        public List<GetInstrumentListItemResponse> Instruments { get; set; }
    }

    public class GetInstrumentListItemResponse
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
