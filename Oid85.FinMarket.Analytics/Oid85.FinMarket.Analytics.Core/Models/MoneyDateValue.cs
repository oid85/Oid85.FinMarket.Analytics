namespace Oid85.FinMarket.Analytics.Core.Models
{
    public class MoneyDateValue
    {
        public DateOnly Date { get; set; }
        public double Money { get; set; }
        public string PortfolioName { get; set; } = string.Empty;
    }
}
