using LiturgyGeek.Calendars.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Engine
{
    public static class ChurchCalendarExtensions
    {
        public static IEnumerable<string> GetAllOccasionCodes(this ChurchCalendar churchCalendar)
        {
            if (churchCalendar.Events.Any(e => e.AttachedEvents.Any() || e.AttachedSeasons.Any()))
            {
                throw new InvalidOperationException(
                        "GetAllOccasionCodes(ChurchCalendar)) can only be called on a preprocessed calendar.");
            }

            foreach (var season in churchCalendar.Seasons)
                yield return season.Key;

            foreach (var churchEvent in churchCalendar.Events.Where(e => e.OccasionCode != null))
                yield return churchEvent.OccasionCode;
        }

        public static void Preprocess(this ChurchCalendar churchCalendar, CalendarReader calendarReader)
        {
            var movableEventQueue = new List<ChurchEvent>();
            var fixedEventQueue = new List<ChurchEvent>();

            // we need this because this method modifies the Events collection
            var topLevelEvents = churchCalendar.Events.ToArray();
            foreach (var churchEvent in topLevelEvents)
            {
                foreach (var attachedSeasonLine in churchEvent.AttachedSeasons)
                {
                    calendarReader.ParseSeasonLine(attachedSeasonLine, out var attachedSeasonCode, out var attachedSeason);
                    attachedSeason.AttachedTo = churchEvent.OccasionCode;

                    churchCalendar.Seasons.Add(attachedSeasonCode, attachedSeason);
                }
                churchEvent.AttachedSeasons.Clear();

                foreach (var attachedEventLine in churchEvent.AttachedEvents.AsEnumerable())
                {
                    var attachedEvent = calendarReader.ParseEventLine(attachedEventLine);
                    attachedEvent.AttachedTo = churchEvent.OccasionCode;

                    if (attachedEvent.Dates[0].IsMovable)
                        movableEventQueue.Add(attachedEvent);
                    else
                        fixedEventQueue.Add(attachedEvent);
                }
                // adding movable events at the end makes the attached movable events appear after standalone movable events
                churchCalendar.Events.AddRange(movableEventQueue);
                // inserting fixed events at the beginning makes the attached fixed events appear before standalone fixed events
                churchCalendar.Events.InsertRange(0, fixedEventQueue);

                churchEvent.AttachedEvents.Clear();
                movableEventQueue.Clear();
                fixedEventQueue.Clear();
            }

            foreach (var churchEvent in churchCalendar.Events)
                churchEvent.EventRankCode ??= churchCalendar.DefaultEventRankCode;
        }
    }
}
