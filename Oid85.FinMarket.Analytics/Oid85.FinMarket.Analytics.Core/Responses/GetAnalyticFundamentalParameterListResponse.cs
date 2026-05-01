using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetAnalyticFundamentalParameterListResponse
    {
        public int TotalCount { get; set; }
        public int NoFillDataCount { get; set; }
        public string NoFillDataTickers { get; set; }
        public List<GetAnalyticFundamentalParameterListItemResponse> FundamentalParameters { get; set; } = [];
    }

    public class GetAnalyticFundamentalParameterListItemResponse
    {
        public int Number { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
        public bool InPortfolio { get; set; }
        public bool FillData { get; set; }
        public FundamentalScore? Score { get; set; } = null;
        public double? BenchmarkChange { get; set; }
        public string? Concept { get; set; } = string.Empty;
        public List<string> Periods { get; set; } = [];
        public List<double?> Price { get; set; } = [];
        public List<double?> NumberShares { get; set; } = [];
        public List<double?> Ebitda { get; set; } = [];
        public List<double?> Ev { get; set; } = [];
        public List<double?> MarketCap { get; set; } = [];
        public List<double?> Dividend { get; set; } = [];
        public List<Parameter<double?>?> Pe { get; set; } = [];
        public List<Parameter<double?>?> Pbv { get; set; } = [];        
        public List<Parameter<double?>?> Revenue { get; set; } = [];
        public List<Parameter<double?>?> NetProfit { get; set; } = [];
        public List<Parameter<double?>?> Eps { get; set; } = [];
        public List<Parameter<double?>?> Fcf { get; set; } = [];        
        public List<Parameter<double?>?> NetDebt { get; set; } = [];        
        public List<Parameter<double?>?> DividendYield { get; set; } = [];        
        public List<Parameter<double?>?> Roa { get; set; } = [];
        public List<Parameter<double?>?> Roe { get; set; } = [];
        public List<Parameter<double?>?> EvEbitda { get; set; } = [];
        public List<Parameter<double?>?> NetDebtEbitda { get; set; } = [];
        public List<Parameter<double?>?> EbitdaRevenue { get; set; } = [];
        public List<Parameter<double?>?> DeltaMinMax { get; set; } = [];        
    }
}
