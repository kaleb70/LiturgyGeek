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

        // GET <CalendarController>/OrthodoxNC/2022/10
        [HttpGet("{custom}/{year}/{month}")]
        public CalendarMonth Get(string custom, int year, int month)
        {
            var cultureInfo = CultureInfo.InvariantCulture;
            var firstOfMonth = new DateTime(year, month, 1);
            var firstDayOfWeek = firstOfMonth.DayOfWeek;

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
                                        .Select(r => new CalendarDayLineItem(r))
                                    .Concat(liturgicalDay.Events.Where(e => (e._MonthViewHeadline ?? false)
                                                                            && e.Name != null)
                                            .Select(e => new CalendarDayLineItem(e)))
                                    .ToArray(),

                        Items = liturgicalDay.Rules.Where(r => r.RuleGroup.Value._MonthViewContent
                                                                && r.Rule.Value.Summary != null
                                                                && r.Show)
                                        .Select(r => new CalendarDayLineItem(r))
                                    .Concat(liturgicalDay.Events.Where(e => (e._MonthViewContent ?? false)
                                                                            && e.Name != null)
                                            .Select(e => new CalendarDayLineItem(e)))
                                    .ToArray(),

                        HeadingClass = string.Join(
                                        " ",
                                        liturgicalDay.Rules.Select(r => r.RuleGroup.Key + "_" + r.Rule.Key)),
                    };
                }

                weeks.Add(new CalendarWeekSummary { Days = days });
            }

            return new CalendarMonth(year, month, cultureInfo.DateTimeFormat.MonthNames[month - 1], weeks.ToArray());
        }
    }
}
