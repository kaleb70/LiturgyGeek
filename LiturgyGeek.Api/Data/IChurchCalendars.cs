using LiturgyGeek.Framework.Calendars;

namespace LiturgyGeek.Api.Data
{
    public interface IChurchCalendars
    {
        ChurchCalendar GetChurchCalendar(string custom);
    }
}
