namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetWeekDeltaResponse
    {
        public List<WeekDeltaHeaderItem> Weeks { get; set; } = [];
        public List<WeekDeltaData> Indexes { get; set; } = [];
        public List<WeekDeltaData> Shares { get; set; } = [];
    }

    public class WeekDeltaData
    {
        public string Ticker { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<WeekDeltaDataItem> Items { get; set; } = [];
        public bool InPortfolio { get; set; }
    }

    public class WeekDeltaHeaderItem
    {
        public int? WeekNumber { get; set; }
        public DateOnly? WeekStartDay { get; set; }
        public DateOnly? WeekEndDay { get; set; }
    }

    public class WeekDeltaDataItem
    {
        public double? Delta { get; set; }
        public double? Price { get; set; }
    }
}
