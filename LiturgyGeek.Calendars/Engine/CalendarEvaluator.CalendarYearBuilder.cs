using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Calendars.Model;
using LiturgyGeek.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Engine
{
    partial class CalendarEvaluator
    {
        private class CalendarYearBuilder
        {
            private readonly CalendarEvaluator evaluator;

            private readonly EvaluationContext context;

            private readonly ChurchCalendar churchCalendar;

            public CalendarYearBuilder(CalendarEvaluator evaluator, EvaluationContext context)
            {
                this.evaluator = evaluator;
                this.context = context;
                churchCalendar = evaluator.churchCalendar;
            }

            public CalendarYear GetCalendarYear(int year)
            {
                var result = new CalendarYear(year);

                FindAllSeasons(result);
                FindAllEvents(result);
                FindAllRules(result);
                FindAllComputedFlags(result);

                return result;
            }

            private void FindAllEvents(CalendarYear calendarYear)
            {
                foreach (var churchEvent in churchCalendar.Events)
                {
                    foreach (var churchDate in churchEvent.Dates)
                    {
                        for (int basisYear = calendarYear.Year - 1; basisYear <= calendarYear.Year + 1; basisYear++)
                        {
                            foreach (var date in context.GetDateInstances(basisYear, churchDate)
                                                    .Where(d => d.Year == calendarYear.Year))
                            {
                                var eventEval = new ChurchEventEval
                                {
                                    Event = churchEvent,
                                    BasisYear = basisYear,
                                    RuleCriteria = Coalesce(basisYear, churchEvent.CommonRules,
                                                            churchEvent.RuleCriteria),
                                };
                                if (churchDate.IsMovable)
                                    calendarYear.Days[date.DayOfYear].MovableEvents.Add(eventEval);
                                else
                                    calendarYear.Days[date.DayOfYear].FixedEvents.Add(eventEval);
                            }
                        }
                    }
                }
            }

            private void FindAllSeasons(CalendarYear calendarYear)
            {
                foreach (var season in churchCalendar.Seasons)
                {
                    for (int basisYear = calendarYear.Year - 1; basisYear <= calendarYear.Year + 1; basisYear++)
                    {
                        var startDate = context.GetSingleDateInstance(basisYear, season.Value.StartDate);
                        var endDate = context.GetSingleDateInstance(basisYear, season.Value.EndDate, season.Value.StartDate);

                        if (startDate?.Year <= calendarYear.Year && endDate?.Year >= calendarYear.Year
                                && startDate <= endDate)
                        {
                            ChurchSeasonEval seasonEval = new ChurchSeasonEval()
                            {
                                SeasonCode = season.Key,
                                Season = season.Value,
                                BasisYear = basisYear,
                                startDate = startDate.Value,
                                endDate = endDate.Value,
                                RuleCriteria = Coalesce(basisYear, season.Value.CommonRules,
                                                        season.Value.RuleCriteria, season.Value.StartDate)
                            };
                            calendarYear.Seasons.Add(seasonEval);
                        }
                    }
                }

                calendarYear.Seasons.Sort(Comparer<ChurchSeasonEval>.Create((x, y) =>
                {
                    return churchCalendar.Seasons[x.SeasonCode].IsDefault ? +1
                            : churchCalendar.Seasons[y.SeasonCode].IsDefault ? -1
                            : x.DaysInSeason - y.DaysInSeason;
                }));

                for (int seasonIndex = 0; seasonIndex < calendarYear.Seasons.Count; seasonIndex++)
                {
                    var season = calendarYear.Seasons[seasonIndex];

                    var minDay = season.startDate.Year < calendarYear.Year
                                    ? 1
                                    : season.startDate.DayOfYear;
                    var maxDay = season.endDate.Year > calendarYear.Year
                                    ? calendarYear.Days.Length
                                    : season.endDate.DayOfYear + 1;

                    for (int dayOfYear = minDay; dayOfYear < maxDay; dayOfYear++)
                        calendarYear.Days[dayOfYear].Seasons.Add(seasonIndex);
                }
            }

            private void FindAllRules(CalendarYear calendarYear)
            {
                for (int dayOfYear = 1; dayOfYear < calendarYear.Days.Length; dayOfYear++)
                {
                    var dayEval = calendarYear.Days[dayOfYear];
                    var date = new DateTime(calendarYear.Year, 1, 1).AddDays(dayOfYear - 1);

                    // Make a flattened collection of all rule criteria for this day, sorted so that the
                    // first match for each rule group wins, according to the following precedence rules:
                    //
                    // 1. Events before Seasons
                    // 2. Event Rank Precedence
                    // 3. Movable Events before Fixed
                    // 4. Criteria Specificity

                    var allEvents = dayEval.MovableEvents.Concat(dayEval.FixedEvents);
                    var allCriteria = allEvents
                                        .OrderBy(e => churchCalendar.EventRanks[e.Event.EventRankCode!].Precedence)
                                        .Select(e => e.RuleCriteria)
                                        .Concat(dayEval.Seasons.Select(s => calendarYear.Seasons[s].RuleCriteria))
                                        .SelectMany(c => c!)
                                        .SelectMany(e => e.Value.OrderByDescending(c => c.Specificity),
                                                    (e, Criteria) => new { RuleGroupCode = e.Key, Criteria });

                    // Where() and GroupBy() are both guaranteed to preserve original order,
                    // so that the sorting code above still does its job.
                    var matchingCriteria = allCriteria
                                            .Where(c => MeetsCriteria(c.Criteria, date, allEvents))
                                            .GroupBy(c => c.RuleGroupCode,
                                                        (RuleGroupCode, c) => new
                                                        {
                                                            RuleGroupCode,
                                                            c.First().Criteria.Criteria.RuleCode,
                                                        });

                    foreach (var match in matchingCriteria)
                        dayEval.Rules.Add(match.RuleGroupCode, match.RuleCode);
                }
            }

            private void FindAllComputedFlags(CalendarYear calendarYear)
            {
                var criteriaEval = churchCalendar.ComputedFlags.Select(
                                    cf => new CriteriaEval<ComputedFlags>(cf, context, calendarYear.Year));

                for (int dayOfYear = 1; dayOfYear < calendarYear.Days.Length; dayOfYear++)
                {
                    var dayEval = calendarYear.Days[dayOfYear];
                    var date = new DateTime(calendarYear.Year, 1, 1).AddDays(dayOfYear - 1);

                    var allEvents = dayEval.MovableEvents.Concat(dayEval.FixedEvents);

                    foreach (var eventEval in allEvents)
                    {
                        var matchingCriteria = criteriaEval
                                                .Where(c => MeetsCriteria(c, date, new[] { eventEval }))
                                                .ToArray();
                        eventEval.AddFlags.AddRange(matchingCriteria.SelectMany(c => c.Criteria.AddFlags));
                        eventEval.RemoveFlags.AddRange(matchingCriteria.SelectMany(c => c.Criteria.RemoveFlags));
                    }
                }
            }

            private Dictionary<string, CriteriaEval<ChurchRuleCriteria>[]> Coalesce(
                    int basisYear,
                    IEnumerable<string> commonCriteria,
                    IEnumerable<KeyValuePair<string, ChurchRuleCriteria[]>> ruleCriteria,
                    ChurchDate? priorDate = null)
            {
                var allCriteria = commonCriteria.SelectMany(c => churchCalendar.CommonRules[c])
                                    .Concat(ruleCriteria);
                return allCriteria
                        .Reverse()
                        .GroupBy(g => g.Key, g => g.Value)
                        .ToDictionary(g => g.Key,
                                        g=> g.First().Select(c => new CriteriaEval<ChurchRuleCriteria>(
                                                        c, context, basisYear, priorDate)).ToArray());
            }

            public bool MeetsCriteria<TCriteria>(CriteriaEval<TCriteria> criteria,
                                                    DateTime date,
                                                    IEnumerable<ChurchEventEval> events)
                    where TCriteria : AbstractCriteria<TCriteria>
            {
                if (criteria.StartDate.HasValue && criteria.StartDate > date)
                    return false;

                if (criteria.EndDate.HasValue && criteria.EndDate < date)
                    return false;

                if (criteria.IncludeDates.Count > 0 && !criteria.IncludeDates.Contains(date))
                    return false;

                if (criteria.Criteria.IncludeRanks.Count > 0
                        && !criteria.Criteria.IncludeRanks.Contains(
                                events.OrderBy(e => churchCalendar.EventRanks[e.Event.EventRankCode!].Precedence)
                                    .FirstOrDefault()
                                    ?.Event.EventRankCode))
                {
                    return false;
                }

                if (criteria.Criteria.IncludeFlags.Count > 0
                        && !criteria.Criteria.IncludeFlags
                            .Intersect(events.SelectMany(e => e.Event.Flags))
                            .Any())
                {
                    return false;
                }

                if (criteria.ExcludeDates.Contains(date))
                    return false;

                if (criteria.Criteria.ExcludeFlags
                        .Intersect(events.SelectMany(e => e.Event.Flags))
                        .Any())
                {
                    return false;
                }

                return true;
            }

        }
    }
}
