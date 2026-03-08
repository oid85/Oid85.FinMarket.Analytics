using Oid85.FinMarket.Analytics.Core.Enums;
using Oid85.FinMarket.Analytics.Core.Responses;

namespace Oid85.FinMarket.Analytics.Application.Helpers
{
    public class TrendStateHelper
    {
        public static (TrendState TrendState, string Message) GetTrendState(List<WeekDeltaDataItem> items)
        {
            var delta1 = items[^1].Delta;
            var delta2 = items[^2].Delta;
            var delta3 = items[^3].Delta;

            if (delta1.HasValue && delta2.HasValue && delta3.HasValue)
            {
                if (delta1 < -2)
                    return (TrendState.BreakTrend, "СЛОМ ТРЕНДА");
            }

            var today = DateTime.Today;

            if (today.DayOfWeek == DayOfWeek.Monday || 
                today.DayOfWeek == DayOfWeek.Tuesday || 
                today.DayOfWeek == DayOfWeek.Wednesday)
            {
                delta1 = items[^2].Delta;
                delta2 = items[^3].Delta;
                delta3 = items[^4].Delta;
            }            

            if (delta1.HasValue && delta2.HasValue && delta3.HasValue)
            {
                if (delta1.Value < -2)
                    return (TrendState.BreakTrend, "СЛОМ ТРЕНДА");

                if (delta1.Value > 0 && delta2.Value > 0 && delta3.Value > 0 || 
                    delta1.Value > 0 && delta2.Value > 0)
                    return (TrendState.StrongTrend, "СИЛЬНЫЙ ТРЕНД");

                if (delta1.Value > 0)
                    return (TrendState.Trend, "ТРЕНД");

                string signal1 = GetSignal(delta1.Value);
                string signal2 = GetSignal(delta2.Value);
                string signal3 = GetSignal(delta3.Value);

                string signal = signal3 + signal2 + signal1;

                List<string> trendSignals = [ 
                    "GGY", "GYG", "YGG",
                    "GYY", "YYG", "YGY"
                    ];
                
                if (trendSignals.Contains(signal))
                    return (TrendState.Trend, "ТРЕНД");
            }

            return (TrendState.NoTrend, "НЕТ ТРЕНДА");

            static string GetSignal(double delta)
            {
                if (delta > 0) return "G"; // Green
                if (delta <= -2) return "R"; // Red
                return "Y"; // Yellow
            }
        }
    }
}

