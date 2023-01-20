using LiturgyGeek.Common.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Dates
{
    public sealed class WeeklyDate : ChurchDate
    {
        public DayOfWeek DayOfWeek { get; private init; }

        public WeeklyDate(DayOfWeek dayOfWeek)
        {
            DayOfWeek = dayOfWeek;
        }

        public override bool Equals(object? obj) => obj is WeeklyDate other && DayOfWeek == other.DayOfWeek;

        public override int GetHashCode() => DayOfWeek.GetHashCode();

        public override string ToString(CultureInfo cultureInfo) => cultureInfo.DateTimeFormat.DayNames[(int)DayOfWeek];

        public override bool IsRecurring => true;

        public override bool IsMovable => false;

        public override DateTime? GetInstance(ChurchCalendarSystem calendarSystem, int year, DateTime? seed = default)
        {
            if (!seed.HasValue)
                return new DateTime(year, 1, 1, calendarSystem.FixedCalendar).First(DayOfWeek);

            var result = seed.Value.AddDays(7);
            return result.Year == year ? result : default(DateTime?);
        }
    }
}
