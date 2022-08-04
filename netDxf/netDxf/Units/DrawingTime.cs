namespace netDxf.Units
{
    using System;

    public static class DrawingTime
    {
        public static TimeSpan EditingTime(double elapsed)
        {
            int days = (int) elapsed;
            double num2 = elapsed - days;
            int hours = (int) (num2 * 24.0);
            num2 -= ((double) hours) / 24.0;
            int minutes = (int) (num2 * 1440.0);
            num2 -= ((double) minutes) / 1440.0;
            double num5 = num2 * 86400.0;
            int seconds = (int) num5;
            return new TimeSpan(days, hours, minutes, seconds, (int) ((num5 - seconds) * 1000.0));
        }

        public static DateTime FromJulianCalendar(double date)
        {
            if ((date < 1721426.0) || (date > 5373484.0))
            {
                throw new ArgumentOutOfRangeException("date", "The valid values range from 1721426 and 5373484 that correspond to January 1, 1 and December 31, 9999 respectively.");
            }
            double num = (int) date;
            double num2 = date - num;
            int num3 = (int) ((num - 1867216.25) / 36524.25);
            num = ((num + 1.0) + num3) - ((int) (((double) num3) / 4.0));
            int num4 = ((int) num) + 0x5f4;
            int num5 = (int) ((num4 - 122.1) / 365.25);
            int num6 = (int) (365.25 * num5);
            int num7 = (int) (((double) (num4 - num6)) / 30.6001);
            int month = (num7 < 14) ? (num7 - 1) : (num7 - 13);
            int year = (month > 2) ? (num5 - 0x126c) : (num5 - 0x126b);
            int day = (num4 - num6) - ((int) (30.6001 * num7));
            int hour = (int) (num2 * 24.0);
            num2 -= ((double) hour) / 24.0;
            int minute = (int) (num2 * 1440.0);
            num2 -= ((double) minute) / 1440.0;
            double num13 = num2 * 86400.0;
            int second = (int) num13;
            return new DateTime(year, month, day, hour, minute, second, (int) ((num13 - second) * 1000.0));
        }

        public static double ToJulianCalendar(DateTime date)
        {
            int num11;
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            double hour = date.Hour;
            double minute = date.Minute;
            double second = date.Second;
            double millisecond = date.Millisecond;
            double num8 = ((day + (hour / 24.0)) + (minute / 1440.0)) + ((second + (millisecond / 1000.0)) / 86400.0);
            if (month < 3)
            {
                year--;
                month += 12;
            }
            int num9 = year / 100;
            int num10 = (2 - num9) + (num9 / 4);
            if (year < 0)
            {
                num11 = (int) ((365.25 * year) - 0.75);
            }
            else
            {
                num11 = (int) (365.25 * year);
            }
            int num12 = (int) (30.6001 * (month + 1));
            return ((((num10 + num11) + num12) + 0x1a42a3) + num8);
        }
    }
}

