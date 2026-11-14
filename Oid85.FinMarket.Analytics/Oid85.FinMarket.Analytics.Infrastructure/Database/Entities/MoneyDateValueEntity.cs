using Oid85.FinMarket.Analytics.Infrastructure.Database.Entities.Base;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Entities
{
    /// <summary>
    /// Деньги
    /// </summary>
    public class MoneyDateValueEntity : BaseEntity
    {
        public DateOnly Date { get; set; }
        public double Money { get; set; }
        public string PortfolioName { get; set; } = string.Empty;
    }
}
