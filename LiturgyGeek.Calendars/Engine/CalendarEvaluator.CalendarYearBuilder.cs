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

            public CalendarYearBuilder(CalendarEvaluator evaluator)
            {
                this.evaluator = evaluator;
            }

            public CalendarYear GetCalendarYear(EvaluationContext context, int year)
            {
                var result = new CalendarYear(year);

                FindAllSeasons(context, year, result);
                FindAllEvents(context, year, result);

                return result;
            }

            private void FindAllEvents(EvaluationContext context, int year, CalendarYear calendarYear)
            {
                foreach (var churchEvent in evaluator.churchCalendar.Events)
                {
                    foreach (var churchDate in churchEvent.Dates)
                    {
                        for (int basisYear = year - 1; basisYear <= year + 1; basisYear++)
                        {
                            foreach (var date in context.GetDateInstances(basisYear, churchDate).Where(d => d.Year == year))
                            {
                                var eventInfo = new ChurchEventInfo
                                {
                                    Event = churchEvent,
                                    BasisYear = basisYear,
                                    RuleCriteria = Coalesce(context, basisYear,
                                                            churchEvent.CommonRules, churchEvent.RuleCriteria),
                                };
                                if (churchDate.IsMovable)
                                    calendarYear.Days[date.DayOfYear].MovableEvents.Add(eventInfo);
                                else
                                    calendarYear.Days[date.DayOfYear].FixedEvents.Add(eventInfo);
                            }
                        }
                    }
                }
            }

            private void FindAllSeasons(EvaluationContext context, int year, CalendarYear calendarYear)
            {
                foreach (var season in evaluator.churchCalendar.Seasons)
                {
                    for (int basisYear = year - 1; basisYear <= year + 1; basisYear++)
                    {
                        var startDate = context.GetSingleDateInstance(basisYear, season.Value.StartDate);
                        var endDate = context.GetSingleDateInstance(basisYear, season.Value.EndDate);

                        if (startDate?.Year <= year && endDate?.Year >= year && startDate <= endDate)
                        {
                            ChurchSeasonInfo info = new ChurchSeasonInfo()
                            {
                                SeasonCode = season.Key,
                                Season = season.Value,
                                BasisYear = basisYear,
                                startDate = startDate.Value,
                                endDate = endDate.Value,
                                RuleCriteria = Coalesce(context, basisYear,
                                                        season.Value.CommonRules, season.Value.RuleCriteria,
                                                        season.Value.StartDate)
                            };
                            calendarYear.Seasons.Add(info);
                        }
                    }
                }

                calendarYear.Seasons.Sort(Comparer<ChurchSeasonInfo>.Create((x, y) =>
                {
                    return evaluator.churchCalendar.Seasons[x.SeasonCode].IsDefault ? +1
                            : evaluator.churchCalendar.Seasons[y.SeasonCode].IsDefault ? -1
                            : x.DaysInSeason - y.DaysInSeason;
                }));

                for (int seasonIndex = 0; seasonIndex < calendarYear.Seasons.Count; seasonIndex++)
                {
                    var season = calendarYear.Seasons[seasonIndex];

                    var minDay = season.startDate.Year < year ? 1 : season.startDate.DayOfYear;
                    var maxDay = season.endDate.Year > year ? calendarYear.Days.Length : season.endDate.DayOfYear + 1;

                    for (int dayOfYear = minDay; dayOfYear < maxDay; dayOfYear++)
                        calendarYear.Days[dayOfYear].Seasons.Add(seasonIndex);
                }
            }

            private Dictionary<string, ChurchRuleCriteriaInfo[]> Coalesce(
                    EvaluationContext context,
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
                            ChurchRuleCriteriaInfo info = new ChurchRuleCriteriaInfo()
                            {
                                Criteria = c,
                                StartDate = startDate,
                                EndDate = endDate,
                                IncludeDates = context.GetDateInstances(basisYear, c.IncludeDates, priorDate).ToArray(),
                                ExcludeDates = context.GetDateInstances(basisYear, c.ExcludeDates, priorDate).ToArray(),
                            };
                            return info;
                        }).ToArray());
            }
        }
    }
}
