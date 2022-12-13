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
        public CalendarDaySummary[][]? Days { get; set; }

        public CalendarMonth(int year, int month, string monthName, CalendarDaySummary[][] days)
        {
            Year = year;
            Month = month;
            MonthName = monthName;
            Days = days;
        }
    }
}
