using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Globalization
{
    public abstract class PaschalCalendar
    {
        /// <summary>
        /// The conclusion of the Council of Nicaea marks the official beginning of a standardized Pascha observance.
        /// </summary>
        public virtual DateTime MinSupportedDateTime => new DateTime(325, 8, 31);

        public virtual DateTime MaxSupportedDateTime => DateTime.MaxValue;

        protected virtual void ValidateDateTime(DateTime time, string? paramName = null)
        {
            if (time < MinSupportedDateTime || time > MaxSupportedDateTime)
            {
                string message = $"{GetType().FullName} only supports {MinSupportedDateTime} - {MaxSupportedDateTime}";
                throw paramName == null
                        ? new ArgumentOutOfRangeException(message)
                        : new ArgumentOutOfRangeException(paramName, time, message);
            }
        }

        protected virtual int MinSupportedPaschalYear => MinSupportedDateTime.Year;

        protected virtual int MaxSupportedPaschalYear
        {
            get
            {
                var maxDate = MaxSupportedDateTime;
                return new DateTime(maxDate.Year, 3, 21) >= maxDate
                        ? maxDate.Year - 1
                        : maxDate.Year - 2;
            }
        }

        protected virtual void ValidatePaschalYear(int year, string? paramName = null)
        {
            if (year < MinSupportedPaschalYear || year > MaxSupportedPaschalYear)
            {
                string message = $"{GetType().FullName} only supports Paschal years {MinSupportedPaschalYear} - {MaxSupportedPaschalYear}";
                throw paramName == null
                        ? new ArgumentOutOfRangeException(message)
                        : new ArgumentOutOfRangeException(paramName, year, message);
            }
        }

        public virtual DateTime AddYears(DateTime time, int years)
        {
            ValidateDateTime(time, nameof(time));
            int week = GetWeek(time);
            int day = GetDay(time);
            return ToDateTime(time.Year + years, week, day, time.Hour, time.Minute, time.Second, time.Millisecond);
        }

        public virtual int GetDay(DateTime time)
        {
            ValidateDateTime(time, nameof(time));
            return (int)time.DayOfWeek + 1;
        }

        public virtual int GetDayOfYear(DateTime time)
        {
            ValidateDateTime(time, nameof(time));
            var dayOfYear = (int)(time.Date - FindPascha(time.Year)).TotalDays;
            if (dayOfYear >= 0)
                ++dayOfYear;
            return dayOfYear;
        }

        public virtual int GetWeek(DateTime time)
        {
            ValidateDateTime(time, nameof(time));
            var dayOfYear = GetDayOfYear(time);
            return dayOfYear < 0
                    ? (dayOfYear - 6) / 7
                    : (dayOfYear + 6) / 7;
        }

        public virtual DateTime ToDateTime(int year, int week, int day, int hour, int minute, int second, int millisecond)
        {
            if (week == 0)
                throw new ArgumentException($"{week} is not a valid Paschal week", nameof(week));
            if (week > 0)
                --week;
            var result = FindPascha(year).Add(new TimeSpan(week * 7 + day - 1, hour, minute, second, millisecond));
            ValidateDateTime(result);
            return result;
        }

        public virtual DateTime ToDateTime(int year, int week, DayOfWeek dayOfWeek)
        {
            return ToDateTime(year, week, (int)dayOfWeek + 1);
        }

        public virtual DateTime ToDateTime(int year, int week, int day)
        {
            if (week == 0)
                throw new ArgumentException($"{week} is not a valid Paschal week", nameof(week));
            if (week > 0)
                --week;
            DateTime result = FindPascha(year).AddDays(week * 7 + day - 1);
            ValidateDateTime(result);
            return result;
        }

        public abstract DateTime FindPascha(int year);
    }
}
