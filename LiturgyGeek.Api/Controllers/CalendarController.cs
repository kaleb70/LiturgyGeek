using LiturgyGeek.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using System.Reflection;
using LiturgyGeek.Calendars;
using LiturgyGeek.Calendars.Engine;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LiturgyGeek.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly CalendarManager calendarManager;

        public CalendarController(CalendarManager calendarManager)
        {
            this.calendarManager = calendarManager;
        }

        // GET <CalendarController>/oca/2022/12/25
        [HttpGet("{custom}/{year}/{month}/{day}")]
        public CalendarDay Get(string custom, int year, int month, int day)
        {
            var cultureInfo = CultureInfo.InvariantCulture;

            var date = new DateTime(year, month, day);

            var items = calendarManager.GetCalendarItems(custom, date);

            return new CalendarDay(items.First().Calendar!.TraditionCode, custom, year, month, day,
                                    cultureInfo.DateTimeFormat.MonthNames[month - 1])
            {
                Items = items
                        .Where(i => i.ChurchRule != null || i.Occasion != null)
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => new CalendarDayItemDetail(i))
                        .ToArray(),
            };
        }

        // GET <CalendarController>/oca/2022/12
        [HttpGet("{custom}/{year}/{month}")]
        public CalendarMonth Get(string custom, int year, int month)
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            var firstOfMonth = new DateTime(year, month, 1);
            var firstDayOfWeek = firstOfMonth.DayOfWeek;

            var date = firstOfMonth.AddDays(-(int)firstDayOfWeek);
            var weeksShown = ((firstOfMonth.AddMonths(1) - date).Days + 6) / 7;

            Data.CalendarItem[] items = calendarManager.GetCalendarItems(custom, date, date.AddDays(weeksShown * 7));
            var weeks = items
                        .GroupBy(i => i.Date.Subtract(date).Days / 7)
                        .Select(week => new CalendarWeekSummary(week))
                        .ToArray();

            return new CalendarMonth(items.First().Calendar!.TraditionCode, custom, year, month,
                                    cultureInfo.DateTimeFormat.MonthNames[month - 1],
                                    weeks);
        }
    }
}
