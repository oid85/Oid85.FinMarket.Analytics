namespace Oid85.FinMarket.Analytics.Core.Models
{
    public class BondCoupon
    {
        public string Ticker { get; set; }
        public long CouponNumber { get; set; }
        public int CouponPeriod { get; set; }
        public DateOnly CouponDate { get; set; }
        public double PayOneBond { get; set; }
    }
}
