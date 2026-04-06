using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetAnalyticFundamentalParameterListResponse
    {
        public List<GetAnalyticFundamentalParameterListItemResponse> FundamentalParameters { get; set; } = [];
    }

    public class GetAnalyticFundamentalParameterListItemResponse
    {
        public int Number { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public bool InPortfolio { get; set; }
        public FundamentalScore? Score { get; set; } = null;
        public double? Moex { get; set; } = null;
        public double BenchmarkChange { get; set; }
        public List<string> Periods { get; set; } = [];
        public List<double?> Price { get; set; } = [];
        public List<double?> Pe { get; set; } = [];
        public List<double?> Ebitda { get; set; } = [];
        public List<double?> Revenue { get; set; } = [];
        public List<double?> NetProfit { get; set; } = [];
        public List<double?> Ev { get; set; } = [];
        public List<double?> NetDebt { get; set; } = [];
        public List<double?> MarketCap { get; set; } = [];
        public List<double?> DividendYield { get; set; } = [];
        public List<double?> Dividend { get; set; } = [];
        public List<double?> Roa { get; set; } = [];
        public List<double?> Pbv { get; set; } = [];
        public List<double?> EvEbitda { get; set; } = [];
        public List<double?> NetDebtEbitda { get; set; } = [];
        public List<double?> EbitdaRevenue { get; set; } = [];
        public List<double?> DeltaMinMax { get; set; } = [];
    }
}
