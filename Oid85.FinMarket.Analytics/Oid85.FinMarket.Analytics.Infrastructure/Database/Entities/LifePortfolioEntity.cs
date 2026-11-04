using Oid85.FinMarket.Analytics.Infrastructure.Database.Entities.Base;

namespace Oid85.FinMarket.Analytics.Infrastructure.Database.Entities
{
    public class LifePortfolioEntity : BaseEntity
    {
        public string Name { get; set; }
		public double? Weight { get; set; }
    }
}
