using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Dates
{
    internal static class GeneralParser
    {
        public static DayOfWeek ParseDayOfWeek(string text, CultureInfo cultureInfo)
        {
            var result = cultureInfo.DateTimeFormat.DayNames.IndexOf(text, cultureInfo.CompareInfo.GetStringComparer(CompareOptions.IgnoreCase));

            if (result < 0)
                throw new FormatException();

            return (DayOfWeek)result;
        }
    }
}
