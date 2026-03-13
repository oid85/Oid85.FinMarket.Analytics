namespace Oid85.FinMarket.Analytics.Core.Models
{
    public class LifePortfolioPosition
    {
        public Guid Id { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public bool IsDeleted { get; set; }
    }
}
