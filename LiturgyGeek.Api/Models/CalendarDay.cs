using System.ComponentModel.DataAnnotations;

namespace LiturgyGeek.Api.Models
{
    public class CalendarDay
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int Day { get; set; }

        public string MonthName { get; set; }

        public CalendarDayItemDetail[]? Items { get; set; }

        public CalendarDay(int year, int month, int day, string monthName)
        {
            Year = year;
            Month = month;
            Day = day;
            MonthName = monthName;
        }
    }
}
