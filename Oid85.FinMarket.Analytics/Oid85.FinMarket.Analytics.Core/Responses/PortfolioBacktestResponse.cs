namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class PortfolioBacktestResponse
    {
        public List<PortfolioRebalanceSeries> Series { get; set; } = [];
        public double Yield { get; set; }
        public double MaxDrawdown { get; set; }
        public double DividendSum { get; set; }
        public double MoneySum { get; set; }
    }

    public class PortfolioRebalanceSeries
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string ColorFill { get; set; } = string.Empty;
        public List<PortfolioRebalanceSeriesItem> Data { get; set; } = [];
    }

    public class PortfolioRebalanceSeriesItem
    {
        public DateOnly Date { get; set; }
        public double? Value { get; set; } = null;
    }
}
