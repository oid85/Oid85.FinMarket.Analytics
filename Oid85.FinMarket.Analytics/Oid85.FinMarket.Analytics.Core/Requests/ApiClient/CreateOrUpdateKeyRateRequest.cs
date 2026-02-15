namespace Oid85.FinMarket.Storage.Core.Requests
{
    public class CreateOrUpdateKeyRateRequest
    {
        public DateOnly Date { get; set; }
        public double? Value { get; set; }
    }
}
