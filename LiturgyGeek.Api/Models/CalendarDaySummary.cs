using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LiturgyGeek.Api.Models
{
    public class CalendarDaySummary
    {
        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Day { get; set; }

        [Required]
        public string MonthName { get; set; }

        public CalendarDaySummaryItem[]? Headlines { get; set; }

        public CalendarDaySummaryItem[]? Items { get; set; }

        public string? HeadingClass { get; set; }

        public CalendarDaySummary(int year, int month, int day, string monthName)
        {
            Year = year;
            Month = month;
            Day = day;
            MonthName = monthName;
        }

        public CalendarDaySummary(DateTime date, IEnumerable<Data.CalendarItem> calendarItems)
        {
            Year = date.Year;
            Month = date.Month;
            Day = date.Day;
            MonthName = Month.ToString();

            calendarItems = calendarItems
                            .Where(i => i.ChurchRule != null || i.Occasion != null)
                            .OrderBy(i => i.DisplayOrder);

            var rule = calendarItems.FirstOrDefault(i => i.ChurchRule != null);
            HeadingClass = rule != null ? string.Join(' ', rule.Class) : string.Empty;

            Headlines = calendarItems.Where(i => i.Class.Contains("monthViewHeadline"))
                                        .Select(i => new CalendarDaySummaryItem(i))
                                        .ToArray();

            Items = calendarItems.Where(i => !i.Class.Contains("monthViewHeadline"))
                                        .Select(i => new CalendarDaySummaryItem(i))
                                        .ToArray();
        }
    }
}
