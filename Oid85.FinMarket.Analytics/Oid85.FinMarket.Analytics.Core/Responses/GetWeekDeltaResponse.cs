using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetWeekDeltaResponse
    {
        public List<WeekDeltaHeaderItem> Weeks { get; set; } = [];
        public List<WeekDeltaData> Indexes { get; set; } = [];
        public List<WeekDeltaData> Shares { get; set; } = [];
        public List<WeekDeltaData> Futures { get; set; } = [];
    }

    public class WeekDeltaData
    {
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<WeekDeltaDataItem> Items { get; set; } = [];
        public bool InPortfolio { get; set; } = false;
        public string TrendState { get; set; } = string.Empty;
        public double? FallingFromMax { get; set; } = null;
    }

    public class WeekDeltaHeaderItem
    {
        public int? WeekNumber { get; set; } = null;
        public DateOnly? WeekStartDay { get; set; } = null;
        public DateOnly? WeekEndDay { get; set; } = null;
    }

    public class WeekDeltaDataItem
    {
        public double? Delta { get; set; } = null;
        public double? Price { get; set; } = null;
    }
}
