using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetTrendDynamicResponse
    {
        public List<DateOnly> Dates { get; set; } = [];
        public List<TrendDynamicData> Indexes { get; set; } = [];
        public List<TrendDynamicData> Shares { get; set; } = [];
        public List<TrendDynamicData> Futures { get; set; } = [];
    }

    public class TrendDynamicData
    {
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<TrendDynamicDataItem> Items { get; set; } = [];
        public bool InPortfolio { get; set; }
        public double? DividendYield { get; set; } = null;
        public FundamentalScore? Score { get; set; } = null;
        public Forecast? Forecast { get; set; } = null;
        public Forecast? NataliaBaffetovnaForecast { get; set; } = null;
        public Forecast? FinanceMarkerForecast { get; set; } = null;
        public Forecast? VladProDengiForecast { get; set; } = null;
        public Forecast? PredictNetProfitForecast { get; set; } = null;
        public bool FillData { get; set; } = false;
        public string? Concept { get; set; } = null;
    }

    public class TrendDynamicDataItem
    {
        public DateOnly Date { get; set; }
        public int? Trend { get; set; } = null;
        public double? Delta { get; set; } = null;
        public double? Price { get; set; } = null;
    }
}
