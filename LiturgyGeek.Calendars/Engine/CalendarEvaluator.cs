using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Calendars.Model;
using LiturgyGeek.Common.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Engine
{
    public partial class CalendarEvaluator
    {
        private readonly ChurchCalendar churchCalendar;
        private readonly ChurchCalendarSystem calendarSystem;
        private readonly CalendarReader calendarReader;

        private readonly Dictionary<int, CalendarYear> calendarYears = new Dictionary<int, CalendarYear>();

        public CalendarEvaluator(ChurchCalendar churchCalendar, CalendarReader calendarReader)
        {
            calendarSystem = new ChurchCalendarSystem(
                churchCalendar.SolarReckoning switch
                {
                    CalendarReckoning.Julian => new JulianCalendar(),
                    CalendarReckoning.Gregorian => new GregorianCalendar(),
                    CalendarReckoning.RevisedJulian => new RevisedJulianCalendar(),
                    _ => throw new NotSupportedException($"CalendarReckonining.{churchCalendar.SolarReckoning} is not supported for fixed dates."),
                },
                churchCalendar.PaschalReckoning switch
                {
                    CalendarReckoning.Julian => new JulianPaschalCalendar(),
                    CalendarReckoning.Gregorian => new GregorianPaschalCalendar(),
                    _ => throw new NotSupportedException($"CalendarReckoning.{churchCalendar.PaschalReckoning} is not supported for movable dates."),
                });

            this.churchCalendar = churchCalendar.Clone();
            this.calendarReader = calendarReader;

            FlattenCalendar();
        }

        private void FlattenCalendar()
        {
            var movableEventQueue = new List<ChurchEvent>();
            var fixedEventQueue = new List<ChurchEvent>();

            // we need this because this method modifies the Events collection
            var topLevelEvents = churchCalendar.Events.ToArray();
            foreach (var churchEvent in topLevelEvents)
            {
                foreach (var attachedSeasonLine in churchEvent.AttachedSeasons)
                {
                    calendarReader.ParseSeasonLine(attachedSeasonLine, out var attachedSeasonCode, out var attachedSeason);
                    attachedSeason.AttachedTo = churchEvent.OccasionCode;

                    churchCalendar.Seasons.Add(attachedSeasonCode, attachedSeason);
                }    
                churchEvent.AttachedSeasons.Clear();

                foreach (var attachedEventLine in churchEvent.AttachedEvents.AsEnumerable())
                {
                    var attachedEvent = calendarReader.ParseEventLine(attachedEventLine);
                    attachedEvent.AttachedTo = churchEvent.OccasionCode;

                    if (attachedEvent.Dates[0].IsMovable)
                        movableEventQueue.Add(attachedEvent);
                    else
                        fixedEventQueue.Add(attachedEvent);
                }
                // adding movable events at the end makes the attached movable events appear after standalone movable events
                churchCalendar.Events.AddRange(movableEventQueue);
                // inserting fixed events at the beginning makes the attached fixed events appear before standalone fixed events
                churchCalendar.Events.InsertRange(0, fixedEventQueue);

                churchEvent.AttachedEvents.Clear();
                movableEventQueue.Clear();
                fixedEventQueue.Clear();
            }
        }

        public Data.CalendarItem[] GetCalendarItems(DateTime date)
        {
            var calendarYear = GetCalendarYear(date.Year);
            var dataCalendar = new Data.Calendar()
            {
                CalendarCode = churchCalendar.CalendarCode,
            };
            var dayEval = calendarYear.Days[date.DayOfYear];

            var movableEvents = dayEval.MovableEvents.Where(e => e.IsDisplayable());
            var fixedEvents = dayEval.FixedEvents.Where(e => e.IsDisplayable());

            IEnumerable<Data.CalendarItem> attachedSeasons = dayEval.Seasons.Select(s => calendarYear.Seasons[s])
                .Where(s => s.Season.IsDisplayable(movableEvents.Concat(fixedEvents).Select(e => e.Event)))
                .Select(s => GetCalendarItem(date, dataCalendar, s));

            var result = movableEvents.Select(e => GetCalendarItem(date, dataCalendar, e))
                            .Concat(attachedSeasons)
                            .Concat(fixedEvents.Select(e => GetCalendarItem(date, dataCalendar, e)))
                            .ToArray();

            for (int i = 0; i < result.Length; i++)
                result[i].DisplayOrder = i;

            return result;
        }

        private Data.CalendarItem GetCalendarItem(DateTime date, Data.Calendar dataCalendar, ChurchEventEval eventEval)
        {
            return new Data.CalendarItem()
            {
                Calendar = dataCalendar,
                Date = date,
                DisplayOrder = 0,
                Occasion = new Data.Occasion()
                {
                    OccasionCode = eventEval.Event.OccasionCode,
                    DefaultName = eventEval.Event.OccasionCode,
                },
                Class = eventEval.Event.Flags.ToList(),
            };
        }

        private Data.CalendarItem GetCalendarItem(DateTime date, Data.Calendar dataCalendar, ChurchSeasonEval seasonEval)
        {
            return new Data.CalendarItem()
            {
                Calendar = dataCalendar,
                Date = date,
                DisplayOrder = 0,
                Occasion = new Data.Occasion()
                {
                    OccasionCode = seasonEval.SeasonCode,
                    DefaultName = seasonEval.SeasonCode,
                },
                Class = seasonEval.Season.Flags.ToList(),
            };
        }

        private CalendarYear GetCalendarYear(int year) => GetCalendarYear(new EvaluationContext(this), year);

        private CalendarYear GetCalendarYear(EvaluationContext context, int year)
        {
            if (!calendarYears.TryGetValue(year, out var result))
                calendarYears.Add(year, result = new CalendarYearBuilder(this).GetCalendarYear(context, year));

            return result;
        }

        private class CalendarYear
        {
            public int Year { get; private init; }

            public DayEval[] Days { get; private init; }

            public List<ChurchSeasonEval> Seasons { get; } = new List<ChurchSeasonEval>();

            public CalendarYear(int year)
            {
                Days = new DayEval[new DateTime(year + 1, 1, 1).Subtract(new DateTime(year, 1, 1)).Days + 1];
                for (int dayOfYear = 1; dayOfYear < Days.Length; dayOfYear++)
                    Days[dayOfYear] = new DayEval();
            }
        }

        private class DayEval
        {
            public List<ChurchEventEval> MovableEvents { get; } = new List<ChurchEventEval>();

            public List<ChurchEventEval> FixedEvents { get; } = new List<ChurchEventEval>();

            public List<int> Seasons { get; } = new List<int>();
        }

        private class ChurchEventEval
        {
            public required ChurchEvent Event { get; init; }

            public required int BasisYear { get; init; }

            public required Dictionary<string, ChurchRuleCriteriaEval[]>? RuleCriteria { get; set; }

            public bool IsDisplayable() => !Event.Flags.Contains("hidden");
        }

        private class ChurchSeasonEval
        {
            public required string SeasonCode { get; init; }

            public required ChurchSeason Season { get; init; }

            public required int BasisYear { get; init; }

            public required DateTime startDate { get; init; }

            public required DateTime endDate { get; init; }

            public required Dictionary<string, ChurchRuleCriteriaEval[]>? RuleCriteria { get; set; }

            public int DaysInSeason => endDate.Subtract(startDate).Days + 1;

            public bool IsDisplayable(IEnumerable<ChurchEventEval> coincidingEvents)
            {
                var attachedTo = Season.AttachedTo;
                return attachedTo != null
                        && !Season.Flags.Contains("hidden")
                        && !coincidingEvents.Any(e => e.Event.AttachedTo == attachedTo);
            }
        }

        private class ChurchRuleCriteriaEval
        {
            public required ChurchRuleCriteria Criteria { get; init; }

            public DateTime? StartDate { get; init; }

            public DateTime? EndDate { get; init; }

            public DateTime[] IncludeDates { get; init; } = { };

            public DateTime[] ExcludeDates { get; init; } = { };
        }

    }
}
