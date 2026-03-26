using Oid85.FinMarket.Analytics.Core.Enums;
using Oid85.FinMarket.Analytics.Core.Models;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Helpers
{
    public class TrendStateHelper
    {
        public static (TrendState TrendState, string Message) GetTrendState(List<DateValue<double>> ultimateSmoothers)
        {
            bool trend = ultimateSmoothers[^1].Value > ultimateSmoothers[^2].Value;

            if (trend)
                return (TrendState.Trend, "ТРЕНД");

            else
                return (TrendState.NoTrend, "НЕТ ТРЕНДА");
        }
    }
}

