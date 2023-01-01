using System.ComponentModel.DataAnnotations;

namespace LiturgyGeek.Api.Models
{
    public class CalendarDay
    {
        public string TraditionKey { get; set; }

        public string CalendarKey { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public int Day { get; set; }

        public string MonthName { get; set; }

        public CalendarDayItemDetail[]? Items { get; set; }

        public CalendarDay(string traditionKey, string calendarKey, int year, int month, int day, string monthName)
        {
            TraditionKey= traditionKey;
            CalendarKey= calendarKey;
            Year = year;
            Month = month;
            Day = day;
            MonthName = monthName;
        }
    }
}
