using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Calendars.Model;
using LiturgyGeek.Common.Collections;
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

        private readonly Dictionary<int, CalendarYear> calendarYears = new Dictionary<int, CalendarYear>();

        public CalendarEvaluator(ChurchCalendar churchCalendar)
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
        }

        public Data.CalendarItem[] GetCalendarItems(Data.LiturgyGeekContext dbContext, DateTime date)
        {
            var calendarYear = GetCalendarYear(date.Year);
            var dataCalendar = new Data.Calendar()
            {
                CalendarCode = churchCalendar.CalendarCode,
            };
            var dayEval = calendarYear.Days[date.DayOfYear];

            var rules = dayEval.Rules
                        .Where(r => IsDisplayable(GetAllRuleFlags(r.Key, r.Value), date))
                        .Select(r => GetCalendarItem(dbContext, date, r.Key, r.Value));

            var movableEvents = dayEval.MovableEvents.Where(e => IsDisplayable(e.Event.Flags, date));
            var fixedEvents = dayEval.FixedEvents.Where(e => IsDisplayable(e.Event.Flags, date));

            IEnumerable<Data.CalendarItem> attachedSeasons = dayEval.Seasons.Select(s => calendarYear.Seasons[s])
                .Where(s => s.IsDisplayable(this, date, movableEvents.Concat(fixedEvents)))
                .Select(s => GetCalendarItem(dbContext, date, s));

            var result = rules
                            .Concat(movableEvents.Select(e => GetCalendarItem(dbContext, date, e)))
                            .Concat(attachedSeasons)
                            .Concat(fixedEvents.Select(e => GetCalendarItem(dbContext, date, e)))
                            .ToArray();

            for (int i = 0; i < result.Length; i++)
                result[i].DisplayOrder = i;

            return result;
        }

        private IEnumerable<string> GetAllRuleFlags(string ruleGroupCode, string ruleCode)
        {
            var ruleGroup = churchCalendar.RuleGroups[ruleGroupCode];
            var rule = ruleGroup.Rules[ruleCode];
            return ruleGroup.Flags.Concat(rule.RuleFlags);
        }

        private bool IsDisplayable(IEnumerable<string> flags, DateTime date, bool defaultDisplayable = true)
        {
            bool displayable = defaultDisplayable
                                ? !flags.Contains("hide")
                                : flags.Contains("show");
            return displayable
                    ? !flags.Contains($"hide-{date.DayOfWeek.ToString().ToLower()}")
                    : flags.Contains($"show-{date.DayOfWeek.ToString().ToLower()}");
        }

        private Data.CalendarItem GetCalendarItem(Data.LiturgyGeekContext dbContext, DateTime date, string ruleGroupCode, string ruleCode)
        {
            var automaticFlags = new[]
            {
                ruleGroupCode,
                $"{ruleGroupCode}_{ruleCode}",
            };
            var ruleGroup = churchCalendar.RuleGroups[ruleGroupCode];
            var rule = ruleGroup.Rules[ruleCode];
            return new Data.CalendarItem()
            {
                Calendar = dbContext.Calendars.Single(c => c.CalendarCode == churchCalendar.CalendarCode),
                Date = date,
                DisplayOrder = 0,
                ChurchRule = dbContext.ChurchRules
                                .Single(r => r.RuleGroupCode == ruleGroupCode
                                            && r.RuleCode == ruleCode
                                            && r.Calendar!.CalendarCode == churchCalendar.CalendarCode),
                Class = ruleGroup.Flags.Concat(rule.RuleFlags).Concat(automaticFlags).ToList(),
            };
        }

        private Data.CalendarItem GetCalendarItem(Data.LiturgyGeekContext dbContext, DateTime date, ChurchEventEval eventEval)
        {
            var automaticFlags = new[]
            {
                eventEval.Event.OccasionCode,
                eventEval.Event.EventRankCode!,
            };

            var eventRank = churchCalendar.EventRanks[eventEval.Event.EventRankCode!];

            return new Data.CalendarItem()
            {
                Calendar = dbContext.Calendars.Single(c => c.CalendarCode == churchCalendar.CalendarCode),
                Date = date,
                DisplayOrder = 0,
                Occasion = dbContext.Occasions.Single(o => o.OccasionCode == eventEval.Event.OccasionCode),
                Class = eventEval.Event.Flags.Concat(eventRank.Flags).Concat(automaticFlags).ToList(),
            };
        }

        private Data.CalendarItem GetCalendarItem(Data.LiturgyGeekContext dbContext, DateTime date, ChurchSeasonEval seasonEval)
        {
            var automaticFlags = new[]
            {
                "season",
                seasonEval.SeasonCode,
            };

            return new Data.CalendarItem()
            {
                Calendar = dbContext.Calendars.Single(c => c.CalendarCode == churchCalendar.CalendarCode),
                Date = date,
                DisplayOrder = 0,
                Occasion = dbContext.Occasions.Single(o => o.OccasionCode == seasonEval.SeasonCode),
                Class = seasonEval.Season.Flags.Concat(automaticFlags).ToList(),
            };
        }

        private CalendarYear GetCalendarYear(int year) => GetCalendarYear(new EvaluationContext(this), year);

        private CalendarYear GetCalendarYear(EvaluationContext context, int year)
        {
            if (!calendarYears.TryGetValue(year, out var result))
                calendarYears.Add(year, result = new CalendarYearBuilder(this, context).GetCalendarYear(year));

            return result;
        }

        private class CalendarYear
        {
            public int Year { get; private init; }

            public DayEval[] Days { get; private init; }

            public List<ChurchSeasonEval> Seasons { get; } = new List<ChurchSeasonEval>();

            public CalendarYear(int year)
            {
                Year = year;
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

            public Dictionary<string, string> Rules { get; } = new Dictionary<string, string>();
        }

        private class ChurchEventEval
        {
            public required ChurchEvent Event { get; init; }

            public required int BasisYear { get; init; }

            public required IReadOnlyDictionary<string, ChurchRuleCriteriaEval[]>? RuleCriteria { get; init; }
        }

        private class ChurchSeasonEval
        {
            public required string SeasonCode { get; init; }

            public required ChurchSeason Season { get; init; }

            public required int BasisYear { get; init; }

            public required DateTime startDate { get; init; }

            public required DateTime endDate { get; init; }

            public required IReadOnlyDictionary<string, ChurchRuleCriteriaEval[]>? RuleCriteria { get; init; }

            public int DaysInSeason => endDate.Subtract(startDate).Days + 1;

            public bool IsDisplayable(CalendarEvaluator calendarEvaluator,
                                        DateTime date,
                                        IEnumerable<ChurchEventEval> coincidingEvents)
            {
                bool isAttached = Season.AttachedTo != null;
                return calendarEvaluator.IsDisplayable(Season.Flags, date, isAttached)
                        && !(isAttached && coincidingEvents.Any(e => e.Event.AttachedTo == Season.AttachedTo));
            }
        }

        [Flags]
        private enum ChurchRuleCriteriaSpecificity
        {
            None = 0,

            ExcludeDates = 0b0000_0001,
            ExcludeFlags = 0b0000_0010,
            IncludeDates = 0b0000_0100,
            IncludeRanks = 0b0000_1000,
            IncludeFlags = 0b0001_0000,
        }

        private class ChurchRuleCriteriaEval
        {
            public required ChurchRuleCriteria Criteria { get; init; }

            public DateTime? StartDate { get; init; }

            public DateTime? EndDate { get; init; }

            public IReadOnlyList<DateTime> IncludeDates { get; init; } = ReadOnlyListEx<DateTime>.Empty;

            public IReadOnlyList<DateTime> ExcludeDates { get; init; } = ReadOnlyListEx<DateTime>.Empty;

            public required ChurchRuleCriteriaSpecificity Specificity { get; init; }
        }

    }
}
