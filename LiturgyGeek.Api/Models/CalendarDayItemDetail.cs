using LiturgyGeek.Calendars.Engine;

namespace LiturgyGeek.Api.Models
{
    public class CalendarDayItemDetail
    {
        public string Title { get; set; }

        public string? Elaboration { get; set; }

        public string Class { get; set; }

        public CalendarDayItemDetail(string title, string elaboration, string @class)
        {
            Title = title;
            Elaboration = elaboration;
            Class = @class;
        }

        public CalendarDayItemDetail(Data.CalendarItem calendarItem)
        {
            Title = calendarItem.Occasion?.FormatDefaultName(calendarItem.Date)
                    ?? calendarItem.ChurchRule?.Summary
                    ?? "[missing name]";
            Elaboration = calendarItem.ChurchRule?.Elaboration ?? string.Empty;
            Class = string.Join(' ', calendarItem.Class);
        }
    }
}
