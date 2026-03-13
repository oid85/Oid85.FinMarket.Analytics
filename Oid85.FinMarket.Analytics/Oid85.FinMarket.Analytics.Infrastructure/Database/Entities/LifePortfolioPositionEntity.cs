using Oid85.FinMarket.Analytics.Infrastructure.Database.Entities.Base;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Entities
{
    public class LifePortfolioPositionEntity : BaseEntity
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public bool IsDeleted { get; set; }
    }
}
