using LiturgyGeek.Common.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Dates
{
    public sealed class MonthlyDate : ChurchDate
    {
        public int Day { get; private init; }

        public DayOfWeek? DayOfWeek { get; private init; }

        private readonly int hashCode;

        public MonthlyDate(int day) : this(day, default(DayOfWeek?))
        {
        }

        public MonthlyDate(int day, DayOfWeek dayOfWeek) : this(day, (DayOfWeek?)dayOfWeek)
        {
        }

        internal MonthlyDate(int day, DayOfWeek? dayOfWeek)
        {
            if (day < -31 || day > 31)
                throw new ArgumentOutOfRangeException(nameof(day), "Must be a nonzero value between -31 and 31");
            else if (day == 0)
                throw new ArgumentException("Value must be nonzero", nameof(day));

            if (dayOfWeek.HasValue && !Enum.IsDefined(dayOfWeek.Value))
                throw new ArgumentException("Invalid value", nameof(dayOfWeek));

            Day = day;
            DayOfWeek = dayOfWeek;

            hashCode = HashCode.Combine(Day, DayOfWeek);
        }

        public override bool Equals(object? obj)
        {
            return obj is MonthlyDate other
                    && hashCode == other.hashCode
                    && Day == other.Day
                    && DayOfWeek == other.DayOfWeek;
        }

        public override int GetHashCode() => hashCode;

        public override string ToString(CultureInfo cultureInfo)
        {
            var result = new StringBuilder();
            result.Append("*/");
            result.Append(Day.ToString(cultureInfo));
            if (DayOfWeek.HasValue)
            {
                result.Append('/');
                result.Append(cultureInfo.DateTimeFormat.DayNames[(int)DayOfWeek]);
            }
            return result.ToString();
        }

        public override bool IsRecurring => true;

        public override bool IsMovable => false;

        public override DateTime? GetInstance(ChurchCalendarSystem calendarSystem, int year, DateTime? seed = default)
        {
            DateTime result;
            if (Day < 0)
            {
                DateTime basis = seed.HasValue
                                    ? new DateTime(seed.Value.Year, seed.Value.Month, 1).AddMonths(2)
                                    : new DateTime(year, 2, 1);

                while (basis.AddDays(-1).Day < -Day)
                    basis = basis.AddMonths(1);

                result = basis.AddDays(Day);
            }
            else
            {
                DateTime basis = seed.HasValue
                                    ? new DateTime(seed.Value.Year, seed.Value.Month, 1).AddMonths(1)
                                    : new DateTime(year, 1, 1);

                while (basis.AddDays(Day - 1).Month != basis.Month)
                    basis = basis.AddMonths(1);

                result = basis.AddDays(Day);
            }

            if (DayOfWeek.HasValue)
            {
                var adjusted = result.First(DayOfWeek.Value);
                result = adjusted.Month == result.Month ? adjusted : default;
            }

            return result.Year == year ? result : default;
        }
    }
}
