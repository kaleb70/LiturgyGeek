using LiturgyGeek.Calendars.Engine;
using LiturgyGeek.Calendars.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars
{
    public class CalendarManager
    {
        private readonly Data.LiturgyGeekContext dbContext;

        public CalendarManager(Data.LiturgyGeekContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private IQueryable<Data.CalendarItem> GetDbCalendarItems(
                string calendarCode,
                DateTime minDate,
                DateTime maxDate,
                bool includeRelationships = true)
        {
            IQueryable<Data.CalendarItem> result = dbContext.CalendarItems
                                    .Where(i => i.Calendar!.CalendarCode == calendarCode
                                                && i.Date >= minDate
                                                && i.Date < maxDate);

            if (includeRelationships)
            {
                result = result.Include(i => i.Calendar)
                                .Include(i => i.ChurchRule)
                                .Include(i => i.Occasion);
            }
            return result;
        }

        public Data.CalendarItem[] GetCalendarItems(string calendarCode, DateTime minDate, DateTime maxDate)
        {
            var daysCount = maxDate.Subtract(minDate).Days;
            var result = GetDbCalendarItems(calendarCode, minDate, maxDate).ToArray();

            if (result.Select(i => i.Date).Distinct().Count() < daysCount)
            {
                var datesNeeded = Enumerable.Range(0, daysCount)
                                    .Select(i => minDate.AddDays(i))
                                    .Where(d => !result.Any(r => r.Date == d));

                var churchCalendar = JsonSerializer.Deserialize<ChurchCalendar>(
                        dbContext.Calendars.Where(c => c.CalendarCode == calendarCode)
                                            .Select(c => c.CalendarDefinition!.Definition)
                                            .Single())!;
                var evaluator = new CalendarEvaluator(churchCalendar);

                dbContext.CalendarItems.AddRange(datesNeeded
                                                .SelectMany(d => evaluator.GetCalendarItems(dbContext, d)));

                dbContext.SaveChanges();

                result = GetDbCalendarItems(calendarCode, minDate, maxDate).ToArray();
            }

            return result;
        }

        public Data.CalendarItem[] GetCalendarItems(string calendarCode, DateTime date)
            => GetCalendarItems(calendarCode, date, date.AddDays(1));
    }
}
