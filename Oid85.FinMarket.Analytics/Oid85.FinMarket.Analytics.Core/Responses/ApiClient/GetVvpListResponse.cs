namespace Oid85.FinMarket.Analytics.Core.Responses.ApiClient
{
    public class GetVvpListResponse
    {
        public GetVvpListResult Result { get; set; } = new();
    }

    public class GetVvpListResult
    {
        public List<GetVvpListItemResponse> Vvps { get; set; } = [];
    }

    public class GetVvpListItemResponse
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public double? Delta { get; set; }
    }
}
