using Oid85.FinMarket.Analytics.Common.KnownConstants;
using Oid85.FinMarket.Analytics.Common.Utils;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Helpers
{
    public class MoneyStatisticsHelper
    {
        public static MoneyStatistics GetMoneyStatisticsAsync(List<MoneyDateValue> moneyDateValues)
        {            
            var yearValues = moneyDateValues.Where(x => x.Date >= DateOnly.FromDateTime(DateTime.Today.AddDays(-365))).OrderBy(x => x.Date).ToList();
            var quarterValues = moneyDateValues.Where(x => x.Date >= DateOnly.FromDateTime(DateTime.Today.AddDays(-90))).OrderBy(x => x.Date).ToList();
            var monthValues = moneyDateValues.Where(x => x.Date >= DateOnly.FromDateTime(DateTime.Today.AddDays(-30))).OrderBy(x => x.Date).ToList();
            var weekValues = moneyDateValues.Where(x => x.Date >= DateOnly.FromDateTime(DateTime.Today.AddDays(-7))).OrderBy(x => x.Date).ToList();

            var moneyStatistics = new MoneyStatistics();

            (moneyStatistics.YearYield, moneyStatistics.YearYieldPercent) = GetYield(yearValues);
            (moneyStatistics.QuarterYield, moneyStatistics.QuarterYieldPercent) = GetYield(quarterValues);
            (moneyStatistics.MonthYield, moneyStatistics.MonthYieldPercent) = GetYield(monthValues);
            (moneyStatistics.WeekYield, moneyStatistics.WeekYieldPercent) = GetYield(weekValues);

            moneyStatistics.EquitySeries =
            [
                new DiagramSeries
                {
                    Name = "Капитал",
                    Color = KnownColors.DarkGreen,
                    ColorFill = KnownColors.Green,
                    Data = [.. moneyDateValues
                        .Where(x => x.Date >= DateOnly.FromDateTime(DateTime.Today.AddDays(-90))).OrderBy(x => x.Date)
                        .Select(x => new DateValue<double?> { Date = x.Date, Value = x.Money })]
                }
            ];

            return moneyStatistics;
        }

        private static (double Yield, double YieldPercent) GetYield(List<MoneyDateValue> moneyDateValues)
        {
            var first = moneyDateValues[0].Money;
            var last = moneyDateValues[^1].Money;

            var yield = last - first;
            var yieldPercent = yield / first * 100.0;

            return (yield.RoundTo(2), yieldPercent.RoundTo(2));
        }
    }
}

