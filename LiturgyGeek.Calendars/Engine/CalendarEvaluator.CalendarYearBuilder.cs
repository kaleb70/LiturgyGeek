using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Calendars.Model;
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

            public CalendarYearBuilder(CalendarEvaluator evaluator, EvaluationContext context)
            {
                this.evaluator = evaluator;
                this.context = context;
            }

            public CalendarYear GetCalendarYear(int year)
            {
                var result = new CalendarYear(year);

                FindAllSeasons(result);
                FindAllEvents(result);

                return result;
            }

            private void FindAllEvents(CalendarYear calendarYear)
            {
                foreach (var churchEvent in evaluator.churchCalendar.Events)
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
                foreach (var season in evaluator.churchCalendar.Seasons)
                {
                    for (int basisYear = calendarYear.Year - 1; basisYear <= calendarYear.Year + 1; basisYear++)
                    {
                        var startDate = context.GetSingleDateInstance(basisYear, season.Value.StartDate);
                        var endDate = context.GetSingleDateInstance(basisYear, season.Value.EndDate);

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
                    return evaluator.churchCalendar.Seasons[x.SeasonCode].IsDefault ? +1
                            : evaluator.churchCalendar.Seasons[y.SeasonCode].IsDefault ? -1
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

            private Dictionary<string, ChurchRuleCriteriaEval[]> Coalesce(
                    int basisYear,
                    IEnumerable<string> commonCriteria,
                    IEnumerable<KeyValuePair<string, ChurchRuleCriteria[]>> ruleCriteria,
                    ChurchDate? priorDate = null)
            {
                var allCriteria = commonCriteria.SelectMany(c => evaluator.churchCalendar.CommonRules[c])
                                    .Concat(ruleCriteria);
                return allCriteria
                        .Reverse()
                        .GroupBy(g => g.Key, g => g.Value)
                        .ToDictionary(g => g.Key, g=> g.First().Select(c =>
                        {
                            var startDate = context.GetSingleDateInstance(basisYear, c.StartDate, priorDate);
                            var endDate = context.GetSingleDateInstance(basisYear, c.EndDate, priorDate);
                            ChurchRuleCriteriaEval criteriaEval = new ChurchRuleCriteriaEval()
                            {
                                Criteria = c,
                                StartDate = startDate,
                                EndDate = endDate,
                                IncludeDates = context.GetDateInstances(basisYear, c.IncludeDates, priorDate).ToArray(),
                                ExcludeDates = context.GetDateInstances(basisYear, c.ExcludeDates, priorDate).ToArray(),
                            };
                            return criteriaEval;
                        }).ToArray());
            }
        }
    }
}
