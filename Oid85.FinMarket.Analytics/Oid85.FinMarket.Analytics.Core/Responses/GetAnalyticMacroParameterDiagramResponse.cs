namespace Oid85.FinMarket.Analytics.Core.Responses
{
    public class GetAnalyticMacroParameterDiagramResponse
    {
        public List<AnalyticMacroParameterSeries> KeyRateSeries { get; set; } = [];
        public List<AnalyticMacroParameterSeries> VvpSeries { get; set; } = [];
        public List<AnalyticMacroParameterSeries> MoneyAggregatesCpiSeries { get; set; } = [];
        public List<AnalyticMacroParameterSeries> MoexSeries { get; set; } = [];
    }

    public class AnalyticMacroParameterSeries
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string ColorFill { get; set; } = string.Empty;
        public List<AnalyticMacroParameterSeriesItem> Data { get; set; } = [];
    }

    public class AnalyticMacroParameterSeriesItem
    {
        public DateOnly Date { get; set; }
        public double? Value { get; set; } = null;
    }
}
