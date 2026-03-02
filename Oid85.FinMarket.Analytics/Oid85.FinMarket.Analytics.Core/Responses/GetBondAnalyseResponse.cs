namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetBondAnalyseResponse
    {
        public List<GetBondAnalyseItemResponse> Items { get; set; } = [];
    }

    public class GetBondAnalyseItemResponse
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Nkd { get; set; }
        public double Yield { get; set; }
        public int DaysToMaturity { get; set; }
        public List<GetBondAnalyseCouponData> Coupons { get; set; } = [];
    }

    public class GetBondAnalyseCouponData
    {
        public DateOnly Date { get; set; }
        public double? CouponSum { get; set; } = null;
    }
}
