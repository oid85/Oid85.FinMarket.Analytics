namespace Oid85.FinMarket.Analytics.Core.Requests.ApiClient
{
    public class CreateOrUpdateKeyRateRequest
    {
        public DateOnly Date { get; set; }
        public double? Value { get; set; }
    }
}
