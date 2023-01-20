using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Globalization
{
    /// <summary>
    /// Represents the Revised Julian calendar.
    /// </summary>
    /// <remarks>
    /// This class is implemented by delegation to <see cref="GregorianCalendar"/>.
    /// Therefore, it only supports dates within the current convergence of the two calendars,
    /// from March 1, A.D. 1600, through February 28, A.D. 2800.
    /// </remarks>
    public class RevisedJulianCalendar : Calendar
    {
        private class RestrictedGregorianCalendar : GregorianCalendar
        {
            public override DateTime MinSupportedDateTime => new DateTime(1600, 3, 1);

            public override DateTime MaxSupportedDateTime => new DateTime(2800, 2, 28, 23, 59, 59, 999);
        }

        private readonly RestrictedGregorianCalendar gregorian = new RestrictedGregorianCalendar();

        /// <summary>
        /// Gets the earliest date and time supported by the <see cref="RevisedJulianCalendar"/> type.
        /// </summary>
        /// <returns>
        /// The earliest date and time supported by the <see cref="RevisedJulianCalendar"/> type,
        /// which is the first moment of March 1, A.D. 1600, the first day of the
        /// current convergence of the Gregorian and Revised Julian calendars.
        /// </returns>
        public override DateTime MinSupportedDateTime => gregorian.MinSupportedDateTime;

        /// <summary>
        /// Gets the latest date and time supported by the <see cref="RevisedJulianCalendar"/> type.
        /// </summary>
        /// <returns>
        /// The latest date and time supported by the <see cref="RevisedJulianCalendar"/> type,
        /// which is the last moment of February 28, A.D. 2800, the last day of the
        /// current convergence of the Gregorian and Revised Julian calendars.
        /// </returns>
        public override DateTime MaxSupportedDateTime => gregorian.MaxSupportedDateTime;

        public override int[] Eras => gregorian.Eras;

        public override DateTime AddMonths(DateTime time, int months) => gregorian.AddMonths(time, months);

        public override DateTime AddYears(DateTime time, int years) => gregorian.AddYears(time, years);

        public override int GetDayOfMonth(DateTime time) => gregorian.GetDayOfMonth(time);

        public override DayOfWeek GetDayOfWeek(DateTime time) => gregorian.GetDayOfWeek(time);

        public override int GetDayOfYear(DateTime time) => gregorian.GetDayOfYear(time);

        public override int GetDaysInMonth(int year, int month, int era) => gregorian.GetDaysInMonth(year, month, era);

        public override int GetDaysInYear(int year, int era) => gregorian.GetDaysInYear(year, era);

        public override int GetEra(DateTime time) => gregorian.GetEra(time);

        public override int GetMonth(DateTime time) => gregorian.GetMonth(time);

        public override int GetMonthsInYear(int year, int era) => gregorian.GetMonthsInYear(year, era);

        public override int GetYear(DateTime time) => gregorian.GetYear(time);

        public override bool IsLeapDay(int year, int month, int day, int era) => gregorian.IsLeapDay(year, month, day, era);

        public override bool IsLeapMonth(int year, int month, int era) => gregorian.IsLeapMonth(year, month, era);

        public override bool IsLeapYear(int year, int era) => gregorian.IsLeapYear(year, era);

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
            => gregorian.ToDateTime(year, month, day, hour, minute, second, millisecond, era);
    }
}
