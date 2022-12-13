using LiturgyGeek.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using LiturgyGeek.Framework.Globalization;
using LiturgyGeek.Framework.Calendars;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LiturgyGeek.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        // GET <CalendarController>/OrthodoxNC/2022/10
        [HttpGet("{custom}/{year}/{month}")]
        public CalendarMonth Get(string custom, int year, int month)
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            var firstOfMonth = new DateTime(year, month, 1);
            var firstDayOfWeek = firstOfMonth.DayOfWeek;

            var days = new List<CalendarDaySummary[]>();
            var date = firstOfMonth.AddDays(-(int)firstDayOfWeek);
            while (date < firstOfMonth.AddMonths(1))
            {
                var week = new CalendarDaySummary[7];
                for (int weekday = 0; weekday < 7; weekday++, date = date.AddDays(1))
                    week[weekday] = new CalendarDaySummary(date.Year, date.Month, date.Day, cultureInfo.DateTimeFormat.MonthNames[date.Month - 1]);

                days.Add(week);
            }

            return new CalendarMonth(year, month, cultureInfo.DateTimeFormat.MonthNames[month - 1], days.ToArray());
        }
    }
}
