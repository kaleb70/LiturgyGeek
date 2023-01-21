using LiturgyGeek.Common;
using LiturgyGeek.Common.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Dates
{
    public sealed class FixedDate : ChurchDate
    {
        public class DayOfWeekFlags
        {
            private readonly FixedDate outer;

            internal DayOfWeekFlags(FixedDate outer)
            {
                this.outer = outer;
            }

            public bool this[DayOfWeek dayOfWeek] => outer._allowedDaysOfWeek[(int)dayOfWeek];
        }

        public int Month { get; private init; }

        public int Day { get; private init; }

        public DayOfWeekFlags AllowedDaysOfWeek { get; private init; }

        public DayOfWeek? AbsoluteDayOfWeek { get; private init; }

        public int? Window { get; private init; }

        private readonly bool[] _allowedDaysOfWeek = new bool[7];

        private readonly int hashCode;

        public FixedDate(int month, int day) : this(month, day, default, default, default)
        {
        }

        public FixedDate(int month, int day, DayOfWeek absoluteDayOfWeek) : this(month, day, absoluteDayOfWeek, default, default)
        {
        }

        public FixedDate(int month, int day, DayOfWeek startDayOfWeek, DayOfWeek endDayOfWeek) : this(month, day, startDayOfWeek, endDayOfWeek, default)
        {
        }

        public FixedDate(int month, int day, DayOfWeek dayOfWeek, int window) : this(month, day, (DayOfWeek?)dayOfWeek, default, window)
        {
        }

        internal FixedDate(int month, int day, DayOfWeek? startDayOfWeek, DayOfWeek? endDayOfWeek, int? window)
        {
            AllowedDaysOfWeek = new DayOfWeekFlags(this);

            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month));
            switch (month)
            {
                case 9:
                case 4:
                case 6:
                case 11:
                    if (day < 0 || day > 30)
                        throw new ArgumentOutOfRangeException(nameof(day));
                    break;

                case 2:
                    if (day < 0 || day > 29)
                        throw new ArgumentOutOfRangeException(nameof(day));
                    break;

                case 3:
                    if (day == -1)
                        break;
                    goto default;

                default:
                    if (day < 0 || day > 31)
                        throw new ArgumentOutOfRangeException(nameof(day));
                    break;
            }

            if (startDayOfWeek.HasValue && !Enum.IsDefined(startDayOfWeek.Value))
                throw new ArgumentException("Invalid value", nameof(startDayOfWeek));

            if (endDayOfWeek.HasValue && !startDayOfWeek.HasValue)
                throw new NotSupportedException($"{nameof(endDayOfWeek)} is only valid if there is a {nameof(startDayOfWeek)}");

            if (endDayOfWeek.HasValue && !Enum.IsDefined(endDayOfWeek.Value))
                throw new ArgumentException("Invalid value", nameof(endDayOfWeek));

            if (window.HasValue && (endDayOfWeek.HasValue || !startDayOfWeek.HasValue))
                throw new NotSupportedException("A multi-day window can only be used with an absolute day of the week");

            if (window < 1 || window > 7)
                throw new ArgumentOutOfRangeException(nameof(window), "Must be between 1 and 7");

            Month = month;
            Day = day;
            Window = window;

            if (startDayOfWeek.HasValue)
            {
                if (endDayOfWeek.HasValue)
                {
                    if (endDayOfWeek < startDayOfWeek)
                    {
                        Array.Fill(_allowedDaysOfWeek, true);
                        Array.Fill(_allowedDaysOfWeek, false, (int)endDayOfWeek + 1, (int)startDayOfWeek - (int)endDayOfWeek - 1);
                    }
                    else
                        Array.Fill(_allowedDaysOfWeek, true, (int)startDayOfWeek, (int)endDayOfWeek - (int)startDayOfWeek + 1);
                }
                else
                {
                    AbsoluteDayOfWeek = startDayOfWeek;
                    _allowedDaysOfWeek[(int)startDayOfWeek] = true;
                }
            }
            else
            {
                Array.Fill(_allowedDaysOfWeek, true);
            }

            var hash = new HashCode();
            hash.Add(Month);
            hash.Add(Day);
            hash.Add(AbsoluteDayOfWeek);
            hash.Add(Window);
            hash.AddArray(_allowedDaysOfWeek);
            hashCode = hash.ToHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj is FixedDate other
                    && hashCode == other.hashCode
                    && Month == other.Month
                    && Day == other.Day
                    && AbsoluteDayOfWeek == other.AbsoluteDayOfWeek
                    && Window == other.Window
                    && _allowedDaysOfWeek.SequenceEqual(other._allowedDaysOfWeek);
        }

        public override int GetHashCode() => hashCode;

        public override string ToString(CultureInfo cultureInfo)
        {
            var result = new StringBuilder();
            result.Append(Month.ToString(cultureInfo));
            result.Append('/');
            result.Append(Day.ToString(cultureInfo));
            if (AbsoluteDayOfWeek.HasValue)
            {
                result.Append('/');
                result.Append(cultureInfo.DateTimeFormat.DayNames[(int)AbsoluteDayOfWeek]);
                if (Window.HasValue)
                {
                    result.Append('/');
                    result.Append(Window.Value.ToString(cultureInfo));
                }
            }
            else if (_allowedDaysOfWeek.Contains(false))
            {
                int start;
                int end;
                if (_allowedDaysOfWeek[0] && _allowedDaysOfWeek[6])
                {
                    int i = 0;
                    while (_allowedDaysOfWeek[i])
                        ++i;
                    end = i - 1;
                    while (!_allowedDaysOfWeek[i])
                        ++i;
                    start = i;
                }
                else
                {
                    int i = 0;
                    while (!_allowedDaysOfWeek[i])
                        ++i;
                    start = i;
                    while (i < 7 && _allowedDaysOfWeek[i])
                        ++i;
                    end = i - 1;
                }
                result.Append(cultureInfo.DateTimeFormat.DayNames[start]);
                result.Append('-');
                result.Append(cultureInfo.DateTimeFormat.DayNames[end]);
            }
            return result.ToString();
        }

        public override bool IsRecurring => false;

        public override bool IsMovable => false;

        public override DateTime? GetInstance(ChurchCalendarSystem calendarSystem, int year, DateTime? seed = default)
        {
            if (seed.HasValue)
                return default;

            DateTime result;

            if (Day == 29 && Month == 2 && !calendarSystem.FixedCalendar.IsLeapYear(year))
            {
                // 2/29 can only be resolved in leap years or if this is a day in a fixed week
                if (!AbsoluteDayOfWeek.HasValue)
                    return default;

                // for the case of a day in a fixed week, the start of the week moves to 3/1
                result = new DateTime(year, 3, 1, calendarSystem.FixedCalendar);
            }
            else
            {
                result = Day == -1 && Month == 3
                            ? new DateTime(year, 3, 1, calendarSystem.FixedCalendar).AddDays(-1)
                            : new DateTime(year, Month, Day, calendarSystem.FixedCalendar);
            }

            if (AbsoluteDayOfWeek.HasValue)
            {
                var adjusted = result.First(AbsoluteDayOfWeek.Value);
                if (Window.HasValue && (adjusted - result).TotalDays >= Window)
                    return default;
                result = adjusted;
            }

            if (!AllowedDaysOfWeek[result.DayOfWeek])
                return default;

            return result;
        }

        public override DateTime? GetInstanceFollowing(ChurchDate? priorDate, ChurchCalendarSystem calendarSystem, int year)
        {
            if (priorDate is FixedDate priorFixedDate
                        && (Month < priorFixedDate.Month
                            || (Month == priorFixedDate.Month && Day < priorFixedDate.Day)))
                ++year;

            return GetInstance(calendarSystem, year);
        }
    }
}
