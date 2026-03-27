using Oid85.FinMarket.Analytics.Core.Enums;
using Oid85.FinMarket.Analytics.Core.Models;

namespace Oid85.FinMarket.Analytics.Application.Helpers
{
    public class TrendStateHelper
    {
        public static (TrendState TrendState, string Message) GetTrendState(List<DateValue<double>> ultimateSmoothers)
        {
            bool upTrend = ultimateSmoothers[^1].Value > ultimateSmoothers[^2].Value;

            bool downTrend =
                ultimateSmoothers[^1].Value < ultimateSmoothers[^2].Value &&
                ultimateSmoothers[^2].Value < ultimateSmoothers[^3].Value &&
                ultimateSmoothers[^3].Value < ultimateSmoothers[^4].Value;

            if (downTrend)
                return (TrendState.DownTrend, "ТРЕНД ВНИЗ");

            if (upTrend)
                return (TrendState.UpTrend, "ТРЕНД ВВЕРХ");

            return (TrendState.NoTrend, "НЕТ ТРЕНДА");
        }
    }
}

