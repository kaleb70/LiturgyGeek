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
        private readonly CalendarReader calendarReader;
        private readonly Data.LiturgyGeekContext dbContext;

        public CalendarManager(CalendarReader calendarReader, Data.LiturgyGeekContext dbContext)
        {
            this.calendarReader = calendarReader;
            this.dbContext = dbContext;
        }

        private IQueryable<Data.CalendarItem> GetDbCalendarItems(string calendarCode, DateTime date,
                                                            bool includeRelationships = true)
        {
            IQueryable<Data.CalendarItem> result = dbContext.CalendarItems
                                    .Where(i => i.Calendar!.CalendarCode == calendarCode && i.Date == date);

            if (includeRelationships)
            {
                result = result.Include(i => i.Calendar)
                                .Include(i => i.ChurchRule)
                                .Include(i => i.Occasion);
            }
            return result;
        }

        public IQueryable<Data.CalendarItem> GetCalendarItems(string calendarCode, DateTime date)
        {
            var result = GetDbCalendarItems(calendarCode, date);

            if (!result.Any())
            {
                var churchCalendar = JsonSerializer.Deserialize<ChurchCalendar>(
                        dbContext.Calendars.Where(c => c.CalendarCode == calendarCode)
                                            .Select(c => c.CalendarDefinition!.Definition)
                                            .Single())!;

                var evaluator = new CalendarEvaluator(churchCalendar);

                dbContext.CalendarItems.AddRange(evaluator.GetCalendarItems(dbContext, date));

                dbContext.SaveChanges();

                result = GetDbCalendarItems(calendarCode, date);
            }

            return result;
        }
    }
}
