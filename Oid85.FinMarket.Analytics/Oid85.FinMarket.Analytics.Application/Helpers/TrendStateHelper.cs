using Oid85.FinMarket.Analytics.Core.Enums;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Helpers
{
    public class TrendStateHelper
    {
        public static TrendState GetTrendState(WeekDeltaData weekDeltaData)
        {
            var delta1 = weekDeltaData.Items[^1].Delta;
            var delta2 = weekDeltaData.Items[^2].Delta;
            var delta3 = weekDeltaData.Items[^3].Delta;

            if (weekDeltaData.Items[^1].Delta == 0)
            {
                delta1 = weekDeltaData.Items[^2].Delta;
                delta2 = weekDeltaData.Items[^3].Delta;
                delta3 = weekDeltaData.Items[^4].Delta;
            }

            if (delta1.HasValue && delta2.HasValue && delta3.HasValue)
            {
                if (delta1 < -2)
                    return TrendState.BreakTrend;

                if (delta1 > 0 && delta2 > 0 && delta3 > 0)
                    return TrendState.StrongTrend;

                if (delta1 > 0)
                    return TrendState.Trend;
            }

            return TrendState.Unknown;
        }
    }
}

