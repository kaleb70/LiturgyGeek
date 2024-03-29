﻿using LiturgyGeek.Common.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Dates
{
    public sealed class MovableDate : ChurchDate
    {
        public int Week { get; private init; }

        public DayOfWeek DayOfWeek { get; private init; }

        private readonly int hashCode;

        public MovableDate(int week, DayOfWeek dayOfWeek)
        {
            Week = week;
            DayOfWeek = dayOfWeek;

            hashCode = HashCode.Combine(Week, DayOfWeek);
        }

        public override bool Equals(object? obj)
        {
            return obj is MovableDate other
                    && hashCode == other.hashCode
                    && Week == other.Week
                    && DayOfWeek == other.DayOfWeek;
        }

        public override int GetHashCode() => hashCode;

        public override string ToString(CultureInfo cultureInfo)
            => Week.ToString(cultureInfo) + '/' + cultureInfo.DateTimeFormat.DayNames[(int)DayOfWeek];

        public override bool IsRecurring => false;

        public override bool IsMovable => true;

        public override DateTime? GetInstance(ChurchCalendarSystem calendarSystem, int year, DateTime? seed = default)
        {
            if (seed.HasValue)
                return null;

            var pascha = calendarSystem.MovableCalendar.FindPascha(year);
            int week = Week > 0 ? Week - 1 : Week;
            return pascha.AddDays(week * 7 + (int)DayOfWeek);
        }

        public override DateTime? GetInstanceFollowing(ChurchDate? priorDate, ChurchCalendarSystem calendarSystem, int year)
        {
            if (priorDate is MovableDate priorMovableDate
                        && (Week < priorMovableDate.Week
                            || (Week == priorMovableDate.Week && DayOfWeek < priorMovableDate.DayOfWeek)))
            {
                // If, ignoring the year, this date would fall before the prior date,
                // then this date range spans multiple years.
                ++year;
            }
            else if (priorDate is FixedDate priorFixedDate)
            {
                // If the prior date is fixed and this date is after Pascha,
                // then the date range is meaningless.
                if (Week > 0)
                    return null;

                // If the prior date falls after Pascha in its calendar year,
                // then this movable date falls before Pascha of the next calendar year.
                if (!priorFixedDate.IsBeforePascha)
                    ++year;
            }

            return GetInstance(calendarSystem, year);
        }
    }
}
