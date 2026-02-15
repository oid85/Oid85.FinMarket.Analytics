namespace Oid85.FinMarket.Analytics.Core.Responses.ApiClient
{
    public class GetConsumerPriceIndexChangeListResponse
    {
        public GetConsumerPriceIndexChangeListResult Result { get; set; } = new();
    }

    public class GetConsumerPriceIndexChangeListResult
    {
        public List<GetConsumerPriceIndexChangeListItemResponse> ConsumerPriceIndexChanges { get; set; } = [];
    }

    public class GetConsumerPriceIndexChangeListItemResponse
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public double? Value { get; set; }
    }
}
