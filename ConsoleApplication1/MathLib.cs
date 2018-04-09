using System;

namespace DataAnalyst
{
    public class MathLib
    {
        public static PriceList ConvertPeriod(PriceList data, Period toPeriod)
        {
            var result = new PriceList() { period = toPeriod, interval = 1};

            if (data.items.Count == 0 || data.period > toPeriod)
            {
                return result;
            }

            if (data.period == toPeriod)
            {
                return data;
            }

            var averagedItem = data.items[0].Clone();
            for (int i = 1; i < data.items.Count; i++)
            {
                if (SamePeriod(averagedItem, data.items[i], toPeriod))
                {
                    averagedItem.High = Math.Max(averagedItem.High, data.items[i].High);
                    averagedItem.Low = Math.Min(averagedItem.Low, data.items[i].Low);
                    averagedItem.Close = data.items[i].Close;
                    averagedItem.Volumn = averagedItem.Volumn + data.items[i].Volumn;
                    averagedItem.Amount = averagedItem.Amount + data.items[i].Amount;
                    averagedItem.Date = data.items[i].Date;
                }
                else
                {
                    averagedItem.Date = GetDateForPeriod(averagedItem.Date, toPeriod);
                    result.items.Add(averagedItem);
                    averagedItem = data.items[i].Clone();
                }
            }

            averagedItem.Date = GetDateForPeriod(averagedItem.Date, toPeriod);
            result.items.Add(averagedItem);

            return result;
        }

        public static PriceList GetAveragePrice(PriceList data, int interval)
        {
            if (data.interval == interval)
            {
                return data;
            }

            var result = new PriceList() { period = data.period, interval = interval};

            var counter = 0;
            var averagedItem = new PriceItem();
            var firstOneAdded = false;

            for (int i = 0; i < data.items.Count; i++)
            {
                if (firstOneAdded)
                {
                    averagedItem = averagedItem.Clone();
                    averagedItem.Date = data.items[i].Date;
                    averagedItem.Close = (averagedItem.Close * interval - data.items[i - interval].Close + data.items[i].Close) / interval;
                    result.items.Add(averagedItem);
                }

                if (counter <= interval - 1)
                {
                    averagedItem.Close = averagedItem.Close + data.items[i].Close;
                    averagedItem.Date = data.items[i].Date;
                    if (counter == interval - 1)
                    {
                        averagedItem.Close = averagedItem.Close / interval;
                        result.items.Add(averagedItem);
                        firstOneAdded = true;
                    }
                }

                counter++;
            }

            return result;
        }

        private static bool SamePeriod(PriceItem item1, PriceItem item2, Period period)
        {
            if (item1.ItemPeriod != item2.ItemPeriod)
            {
                return false;
            }

            switch (period)
            {
                case Period.Day:
                    return item1.Date == item2.Date;
                case Period.Week:
                    return InSameWeek(item1.Date, item2.Date);
                case Period.Month:
                default:
                    return item1.Date.Year == item2.Date.Year && item1.Date.Month == item2.Date.Month;
            }
        }

        public static bool InSameWeek(DateTime d1, DateTime d2)
        {
            DateTime beginningOfWeekDate1 = GetFirstDayOfWeek(d1);
            DateTime beginningOfWeekDate2 = GetFirstDayOfWeek(d2);
            return beginningOfWeekDate1 == beginningOfWeekDate2;
        }

        private static DateTime GetFirstDayOfWeek(DateTime d)
        {
            switch (d.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return d;
                case DayOfWeek.Tuesday:
                    return d.AddDays(-1);
                case DayOfWeek.Wednesday:
                    return d.AddDays(-2);
                case DayOfWeek.Thursday:
                    return d.AddDays(-3);
                case DayOfWeek.Friday:
                    return d.AddDays(-4);
                case DayOfWeek.Saturday:
                    return d.AddDays(-5);
                case DayOfWeek.Sunday:
                    return d.AddDays(-6);
                default:
                    throw new ApplicationException();
            }
        }

        public static DateTime GetDateForPeriod(DateTime date, Period period)
        {
            if (period == Period.Day)
            {
                return date;
            }

            if (period == Period.Week)
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return date.AddDays(4);
                    case DayOfWeek.Tuesday:
                        return date.AddDays(3);
                    case DayOfWeek.Wednesday:
                        return date.AddDays(2);
                    case DayOfWeek.Thursday:
                        return date.AddDays(1);
                    case DayOfWeek.Friday:
                        return date;
                    case DayOfWeek.Saturday:
                        return date.AddDays(-1);
                    case DayOfWeek.Sunday:
                        return date.AddDays(-2);
                }
            }

            if (period == Period.Month)
            {
                if (date.Month == 12)
                {
                    return new DateTime(date.Year, date.Month, 31);
                }

                return new DateTime(date.Year, date.Month + 1, 1).AddDays(-1);
            }

            return date;
        }

        public static DateTime GetPreviousDate(DateTime date, Period period)
        {
            var adjustedDate = GetDateForPeriod(date, period);
            var result = date;

            switch (period)
            {
                case Period.Day:
                    result = adjustedDate.AddDays(-1);
                    while (result.DayOfWeek == DayOfWeek.Saturday || result.DayOfWeek == DayOfWeek.Sunday)
                    {
                        result = date.AddDays(-1);
                    }
                    break;
                case Period.Week:
                    result = adjustedDate.AddDays(-7);
                    break;
                case Period.Month:
                    result = new DateTime(adjustedDate.Year, adjustedDate.Month, 1).AddDays(-1);
                    break;
            }

            return result;
        }
    }
}
