using System.ComponentModel.DataAnnotations;

namespace LiturgyGeek.Api.Models
{
    public class CalendarMonth
    {
        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public string? MonthName { get; set; }

        [Required]
        public CalendarWeekSummary[]? Weeks { get; set; }

        public CalendarMonth(int year, int month, string monthName, CalendarWeekSummary[] weeks)
        {
            Year = year;
            Month = month;
            MonthName = monthName;
            Weeks = weeks;
        }
    }
}
