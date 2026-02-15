namespace Oid85.FinMarket.Storage.Core.Requests
{
    public class CreateOrUpdateMonetaryAggregateRequest
    {
        public DateOnly Date { get; set; }
        public double? M0 { get; set; }
        public double? M1 { get; set; }
        public double? M2 { get; set; }
        public double? M2X { get; set; }
    }
}
