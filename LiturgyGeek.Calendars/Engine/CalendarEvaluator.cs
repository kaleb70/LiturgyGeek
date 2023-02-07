using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Calendars.Model;
using LiturgyGeek.Common.Collections;
using LiturgyGeek.Common.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
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
            var dayEval = calendarYear.Days[date.DayOfYear];

            var rules = dayEval.Rules
                        .Where(r => IsDisplayable(GetAllRuleFlags(r.Key, r.Value), date))
                        .Select(r => GetCalendarItem(dbContext, date, r.Key, r.Value));

            var movableEvents = dayEval.MovableEvents.Where(e => IsDisplayable(e.GetAllFlags(), date));
            var fixedEvents = dayEval.FixedEvents.Where(e => IsDisplayable(e.GetAllFlags(), date));
            var allEvents = movableEvents.Concat(fixedEvents);

            var allSeasons = dayEval.Seasons.Select(s => calendarYear.Seasons[s])
                                .Where(s => s.IsDisplayable(this, date, allEvents))
                                .OrderByDescending(s => (s.endDate-s.startDate).Days);

            var unattachedSeasons = allSeasons.Where(s => s.Season.AttachedTo == null);
            var attachedSeasons = allSeasons.Where(s => s.Season.AttachedTo != null);

            var result = (rules.Any() || allEvents.Any() || attachedSeasons.Any())
                            ? rules
                                .Concat(unattachedSeasons.Select(s => GetCalendarItem(dbContext, date, s)))
                                .Concat(movableEvents.Select(e => GetCalendarItem(dbContext, date, e)))
                                .Concat(attachedSeasons.Select(s => GetCalendarItem(dbContext, date, s)))
                                .Concat(fixedEvents.Select(e => GetCalendarItem(dbContext, date, e)))
                                .ToArray()
                            : new[] { GetFillerCalendarItem(dbContext, date) };

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

        private Data.CalendarItem GetFillerCalendarItem(Data.LiturgyGeekContext dbContext, DateTime date)
        {
            return new Data.CalendarItem()
            {
                Calendar = dbContext.Calendars.Single(c => c.CalendarCode == churchCalendar.CalendarCode),
                Date = date,
                DisplayOrder = 0,
                Class = new(),
                IsAttachedEvent = false,
            };
        }

        private Data.CalendarItem GetCalendarItem(Data.LiturgyGeekContext dbContext, DateTime date, string ruleGroupCode, string ruleCode)
        {
            var automaticFlags = new[]
            {
                $"rule_{ruleGroupCode}",
                $"rule_{ruleGroupCode}_{ruleCode}",
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
                Class = ruleGroup.Flags.Concat(rule.RuleFlags).Concat(automaticFlags).Distinct().ToList(),
                IsAttachedEvent = false,
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
                Class = eventEval.GetAllFlags().Concat(eventRank.Flags).Concat(automaticFlags).ToList(),
                IsAttachedEvent = eventEval.Event.AttachedTo != null,
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
                ReferenceDate = seasonEval.startDate,
                DisplayOrder = 0,
                Occasion = dbContext.Occasions.Single(o => o.OccasionCode == seasonEval.SeasonCode),
                Class = seasonEval.Season.Flags.Concat(automaticFlags).ToList(),
                IsAttachedEvent = seasonEval.Season.AttachedTo != null,
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

            public required IReadOnlyDictionary<string, CriteriaEval<ChurchRuleCriteria>[]>? RuleCriteria { get; init; }

            public HashSet<string> AddFlags { get; } = new HashSet<string>();

            public HashSet<string> RemoveFlags { get; } = new HashSet<string>();

            public IEnumerable<string> GetAllFlags() => Event.Flags.Union(AddFlags).Except(RemoveFlags);
        }

        private class ChurchSeasonEval
        {
            public required string SeasonCode { get; init; }

            public required ChurchSeason Season { get; init; }

            public required int BasisYear { get; init; }

            public required DateTime startDate { get; init; }

            public required DateTime endDate { get; init; }

            public required IReadOnlyDictionary<string, CriteriaEval<ChurchRuleCriteria>[]>? RuleCriteria { get; init; }

            public int DaysInSeason => endDate.Subtract(startDate).Days + 1;

            public bool IsDisplayable(CalendarEvaluator calendarEvaluator,
                                        DateTime date,
                                        IEnumerable<ChurchEventEval> coincidingEvents)
            {
                bool isAttached = Season.AttachedTo != null;
                return calendarEvaluator.IsDisplayable(Season.Flags, date, isAttached)
                        && !(isAttached && coincidingEvents.Any(e => e.Event.AttachedTo == Season.AttachedTo
                                                                    || e.Event.OccasionCode == Season.AttachedTo));
            }
        }

        [Flags]
        private enum CriteriaSpecificity
        {
            None = 0,

            ExcludeDates = 0b0000_0001,
            ExcludeFlags = 0b0000_0010,
            IncludeDates = 0b0000_0100,
            IncludeRanks = 0b0000_1000,
            IncludeFlags = 0b0001_0000,
        }

        private class CriteriaEval<TCriteria> where TCriteria : AbstractCriteria<TCriteria>
        {
            public TCriteria Criteria { get; private init; }

            public DateTime? StartDate { get; private init; }

            public DateTime? EndDate { get; private init; }

            public IReadOnlyList<DateTime> IncludeDates { get; private init; }

            public IReadOnlyList<DateTime> ExcludeDates { get; private init; }

            public CriteriaSpecificity Specificity { get; init; }

            public CriteriaEval(TCriteria criteria, EvaluationContext context, int basisYear, ChurchDate? priorDate = null)
            {
                Criteria = criteria;
                StartDate = context.GetSingleDateInstance(basisYear, criteria.StartDate, priorDate);
                EndDate = context.GetSingleDateInstance(basisYear, criteria.EndDate, priorDate);
                IncludeDates = context.GetDateInstances(basisYear, criteria.IncludeDates, priorDate).ToArray();
                ExcludeDates = context.GetDateInstances(basisYear, criteria.ExcludeDates, priorDate).ToArray();

                Specificity = CriteriaSpecificity.None;
                if (criteria.StartDate != null || criteria.EndDate != null || criteria.ExcludeDates.Count > 0)
                    Specificity |= CriteriaSpecificity.ExcludeDates;
                if (criteria.ExcludeFlags.Count > 0)
                    Specificity |= CriteriaSpecificity.ExcludeFlags;
                if (criteria.IncludeDates.Count > 0)
                    Specificity |= CriteriaSpecificity.IncludeDates;
                if (criteria.IncludeRanks.Count > 0)
                    Specificity |= CriteriaSpecificity.IncludeRanks;
                if (criteria.IncludeFlags.Count > 0)
                    Specificity |= CriteriaSpecificity.IncludeFlags;
            }
        }

    }
}
