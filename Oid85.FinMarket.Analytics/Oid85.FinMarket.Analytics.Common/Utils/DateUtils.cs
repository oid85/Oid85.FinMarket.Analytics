namespace Oid85.FinMarket.Analytics.Common.Utils
{
    public static class DateUtils
    {
        public static List<DateOnly> GetDates(DateOnly from, DateOnly to)
        {
            var curDate = from;
            var dates = new List<DateOnly>();

            while (curDate <= to)
            {
                dates.Add(curDate);
                curDate = curDate.AddDays(1);
            }

            return dates;
        }
    }
}
