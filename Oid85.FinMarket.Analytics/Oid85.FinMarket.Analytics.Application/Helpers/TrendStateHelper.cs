using Oid85.FinMarket.Analytics.Core.Enums;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Helpers
{
    public class TrendStateHelper
    {
        public static (TrendState TrendState, string Message) GetTrendState(List<DateValue<double>> ultimateSmoothers) => 
            ultimateSmoothers[^1].Value > ultimateSmoothers[^2].Value ? (TrendState.Trend, "ТРЕНД") : (TrendState.NoTrend, "НЕТ ТРЕНДА");
    }
}

