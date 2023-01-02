using LiturgyGeek.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using LiturgyGeek.Framework.Globalization;
using LiturgyGeek.Framework.Calendars;
using System.Text.Json;
using System.Reflection;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LiturgyGeek.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly CalendarEvaluator calendarEvaluator;

        public CalendarController(CalendarEvaluator calendarEvaluator)
        {
            this.calendarEvaluator = calendarEvaluator;
        }

        // GET <CalendarController>/oca/2022/12/25
        [HttpGet("{custom}/{year}/{month}/{day}")]
        public CalendarDay Get(string custom, int year, int month, int day)
        {
            var cultureInfo = CultureInfo.InvariantCulture;

            var calendar = calendarEvaluator.GetCalendar(custom);

            var date = new DateTime(year, month, day);
            var liturgicalDay = calendarEvaluator.Evaluate(custom, date, date.AddDays(1)).Single();

            return new CalendarDay(calendar.TraditionKey, calendar.CalendarKey, date.Year, date.Month, date.Day, cultureInfo.DateTimeFormat.MonthNames[date.Month - 1])
            {
                Items = liturgicalDay.Rules.Where(r => r.Rule.Value.Summary != null && r.Show)
                                        .Select(r => new CalendarDayItemDetail(r))
                                    .Concat(liturgicalDay.Events.Where(e => e.Event.Name != null)
                                            .Select(e => new CalendarDayItemDetail(e)))
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

            var calendar = calendarEvaluator.GetCalendar(custom);

            var weeks = new List<CalendarWeekSummary>();
            var date = firstOfMonth.AddDays(-(int)firstDayOfWeek);
            var weeksShown = ((firstOfMonth.AddMonths(1) - date).Days + 6) / 7;
            var liturgicalMonth = calendarEvaluator.Evaluate(custom, date, date.AddDays(weeksShown * 7));

            while (date < firstOfMonth.AddMonths(1))
            {
                var days = new CalendarDaySummary[7];
                for (int weekday = 0; weekday < 7; weekday++, date = date.AddDays(1))
                {
                    var liturgicalDay = liturgicalMonth.Single(d => d.Date == date);

                    days[weekday] = new CalendarDaySummary(date.Year, date.Month, date.Day, cultureInfo.DateTimeFormat.MonthNames[date.Month - 1])
                    {
                        Headlines = liturgicalDay.Rules.Where(r => r.RuleGroup.Value._MonthViewHeadline
                                                                    && r.Rule.Value.Summary != null
                                                                    && r.Show)
                                        .Select(r => new CalendarDaySummaryItem(r))
                                    .Concat(liturgicalDay.Events.Where(e => (e.Event._MonthViewHeadline ?? false)
                                                                            && e.Event.Name != null)
                                            .Select(e => new CalendarDaySummaryItem(e)))
                                    .ToArray(),

                        Items = liturgicalDay.Rules.Where(r => r.RuleGroup.Value._MonthViewContent
                                                                && r.Rule.Value.Summary != null
                                                                && r.Show)
                                        .Select(r => new CalendarDaySummaryItem(r))
                                    .Concat((liturgicalDay.Events.Any(e => (e.Event._MonthViewHeadline ?? false)
                                                                            || (e.Event._MonthViewContent ?? false))
                                                ? liturgicalDay.Events.Where(e => (e.Event._MonthViewContent ?? false)
                                                                                    && e.Event.Name != null)
                                                : liturgicalDay.Events.Where(e => e.Event.Name != null)
                                                                        .Take(1))
                                            .Select(e => new CalendarDaySummaryItem(e)))
                                    .ToArray(),

                        HeadingClass = string.Join(
                                        " ",
                                        liturgicalDay.Rules.Select(r => $"rule_{r.RuleGroup.Key}_{r.Rule.Key}")
                                            .Concat(liturgicalDay.Rules.Select(r => $"rule_{r.RuleGroup.Key}").Distinct())),
                    };
                }

                weeks.Add(new CalendarWeekSummary { Days = days });
            }

            return new CalendarMonth(calendar.TraditionKey, calendar.CalendarKey, year, month, cultureInfo.DateTimeFormat.MonthNames[month - 1], weeks.ToArray());
        }
    }
}
