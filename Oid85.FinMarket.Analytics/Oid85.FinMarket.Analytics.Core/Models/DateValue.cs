namespace Oid85.FinMarket.Analytics.Core.Models
{
    public class DateValue<T>
    {
        public DateOnly Date { get; set; }
        public T Value { get; set; }
    }
}
