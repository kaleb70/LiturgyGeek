using System.ComponentModel.DataAnnotations;

namespace LiturgyGeek.Api.Models
{
    public class CalendarWeekSummary
    {
        public CalendarDaySummary[] Days { get; init; }

        public bool HasHeadlines => Days?.Any(d => d?.Headlines?.Length > 0) ?? false;

        public CalendarWeekSummary(CalendarDaySummary[] days)
        {
            Days = days;
        }

        public CalendarWeekSummary(IEnumerable<Data.CalendarItem> calendarItems)
        {
            Days = calendarItems.GroupBy(i => i.Date)
                                .OrderBy(d => d.Key)
                                .Select(d => new CalendarDaySummary(d.Key, d))
                                .ToArray();
        }
    }
}
