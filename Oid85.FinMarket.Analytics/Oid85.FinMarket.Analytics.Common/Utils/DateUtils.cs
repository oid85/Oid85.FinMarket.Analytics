namespace Oid85.FinMarket.Analytics.Common.Utils
{
    public static class DateUtils
    {
        public static List<DateOnly> GetDates(DateOnly from, DateOnly to)
        {
            var curDate = from;
            var dates = new List<DateOnly>();

            var daysOfWeek = new List<DayOfWeek>()
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday
        };

            while (curDate <= to)
            {
                if (daysOfWeek.Contains(curDate.DayOfWeek))
                    dates.Add(curDate);

                curDate = curDate.AddDays(1);
            }

            return dates;
        }
    }
}
