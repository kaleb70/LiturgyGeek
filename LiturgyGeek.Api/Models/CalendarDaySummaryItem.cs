using LiturgyGeek.Calendars.Engine;
using Microsoft.OpenApi.Services;

namespace LiturgyGeek.Api.Models
{
    public class CalendarDaySummaryItem
    {
        public string Summary { get; set; }

        public string? Elaboration { get; set; }

        public string Class { get; set; }

        public CalendarDaySummaryItem(string summary, string elaboration, string @class)
        {
            Summary = summary;
            Elaboration = elaboration;
            Class = @class;
        }

        public CalendarDaySummaryItem(Data.CalendarItem calendarItem)
        {
            Summary = calendarItem.ChurchRule?.Summary
                        ?? calendarItem.Occasion?.FormatDefaultName(calendarItem.Date)
                        ?? string.Empty;
            Elaboration = calendarItem.ChurchRule?.Elaboration;
            Class = string.Join(' ', calendarItem.Class);
        }
    }
}
