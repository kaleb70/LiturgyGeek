using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Dates
{
    public static class DateTimeExtensions
    {
        public static DateTime First(this DateTime dateTime, DayOfWeek dayOfWeek)
        {
            if (!Enum.IsDefined(dayOfWeek))
                throw new ArgumentException();

            int baseDay = (int)dateTime.DayOfWeek;
            int targetDay = (int)dayOfWeek;
            if (targetDay != baseDay)
            {
                if (targetDay < baseDay)
                    targetDay += 7;
                return dateTime.AddDays(targetDay - baseDay);
            }
            return dateTime;
        }
    }
}
