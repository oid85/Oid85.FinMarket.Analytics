namespace Oid85.FinMarket.Analytics.Core.Responses.ApiClient
{
    public class GetKeyRateListResponse
    {
        public GetKeyRateListResult Result { get; set; } = new();
    }

    public class GetKeyRateListResult
    {
        public List<GetKeyRateListItemResponse> KeyRates { get; set; } = [];
    }

    public class GetKeyRateListItemResponse
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public double? Value { get; set; }
    }
}
