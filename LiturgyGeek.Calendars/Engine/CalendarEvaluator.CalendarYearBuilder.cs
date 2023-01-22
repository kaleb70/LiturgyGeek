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

                foreach (var churchEvent in evaluator.churchCalendar.Events.Select((Value, Index) => new { Value, Index }))
                {
                    foreach (var churchDate in churchEvent.Value.Dates)
                    {
                        for (int basisYear = year - 1; basisYear <= year + 1; basisYear++)
                        {
                            foreach (var date in context.GetDateInstances(basisYear, churchDate).Where(d => d.Year == year))
                            {
                                if (churchDate.IsMovable)
                                    result.Days[date.DayOfYear].MovableEvents.Add(churchEvent.Index);
                                else
                                    result.Days[date.DayOfYear].FixedEvents.Add(churchEvent.Index);
                            }
                        }
                    }
                }

                foreach (var season in evaluator.churchCalendar.Seasons)
                {
                    for (int basisYear = year - 1; basisYear <= year + 1; basisYear++)
                    {
                        var startDate = context.GetSingleDateInstance(basisYear, season.Value.StartDate);
                        var endDate = context.GetSingleDateInstance(basisYear, season.Value.EndDate);

                        if (startDate?.Year <= year && endDate?.Year >= year && startDate <= endDate)
                        {
                            result.Seasons.Add(new SeasonInfo()
                            {
                                SeasonCode = season.Key,
                                startDate = startDate.Value,
                                endDate = endDate.Value,
                            });
                        }
                    }
                }

                result.Seasons.Sort(Comparer<SeasonInfo>.Create((x, y) =>
                {
                    return evaluator.churchCalendar.Seasons[x.SeasonCode].IsDefault ? +1
                            : evaluator.churchCalendar.Seasons[y.SeasonCode].IsDefault ? -1
                            : x.DaysInSeason - y.DaysInSeason;
                }));

                for (int seasonIndex = 0; seasonIndex < result.Seasons.Count; seasonIndex++)
                {
                    var season = result.Seasons[seasonIndex];

                    var minDay = season.startDate.Year < year ? 1 : season.startDate.DayOfYear;
                    var maxDay = season.endDate.Year > year ? result.Days.Length : season.endDate.DayOfYear + 1;

                    for (int dayOfYear = minDay; dayOfYear < maxDay; dayOfYear++)
                        result.Days[dayOfYear].Seasons.Add(seasonIndex);
                }

                return result;
            }
        }
    }
}
