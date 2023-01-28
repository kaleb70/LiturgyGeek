using LiturgyGeek.Calendars.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars
{
    public class CalendarManager
    {
        private readonly CalendarReader calendarReader;

        public CalendarManager(CalendarReader calendarReader)
        {
            this.calendarReader = calendarReader;
        }

        public Data.CalendarItem[] GetCalendarItems(string calendarCode, DateTime date)
        {
            var json = File.ReadAllText($@"Sources\{calendarCode}.json");
            var lines = File.Exists($@"Sources\{calendarCode}.txt")
                        ? File.ReadAllText($@"Sources\{calendarCode}.txt")
                        : null;

            var churchCalendar = calendarReader.Read(json, lines);

            var evaluator = new CalendarEvaluator(churchCalendar);
            return evaluator.GetCalendarItems(date);
        }
    }
}
