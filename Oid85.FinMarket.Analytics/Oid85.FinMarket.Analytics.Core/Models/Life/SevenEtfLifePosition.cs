namespace Oid85.FinMarket.Analytics.Core.Models.Life
{
    public class SevenEtfLifePosition
    {
		public Guid Id { get; set; }
        public string Ticker { get; set; }
        public string Name { get; set; }
        public int? Size { get; set; }
        public double? Price { get; set; }
        public double? Weight { get; set; }
    }
}
