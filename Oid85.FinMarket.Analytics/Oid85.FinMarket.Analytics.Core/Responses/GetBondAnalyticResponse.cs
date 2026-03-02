namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetBondAnalyticResponse
    {
        public List<GetBondAnalyticItemResponse> Items { get; set; } = [];
    }

    public class GetBondAnalyticItemResponse
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Nkd { get; set; }
        public double Yield { get; set; }
        public int DaysToMaturity { get; set; }
        public List<GetBondAnalyticCouponData> Coupons { get; set; } = [];
    }

    public class GetBondAnalyticCouponData
    {
        public DateOnly Date { get; set; }
        public double? CouponSum { get; set; } = null;
    }
}
