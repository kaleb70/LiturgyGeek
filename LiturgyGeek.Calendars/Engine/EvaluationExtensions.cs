using LiturgyGeek.Calendars.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Engine
{
    internal static class EvaluationExtensions
    {
        public static bool IsDisplayable(this ChurchEvent churchEvent)
        {
            return !churchEvent.Flags.Contains("hidden");
        }

        public static bool IsDisplayable(this ChurchSeason season, IEnumerable<ChurchEvent> coincidingEvents)
        {
            return season.AttachedTo != null
                    && !season.Flags.Contains("hidden")
                    && !coincidingEvents.Any(e => e.AttachedTo == season.AttachedTo);
        }
    }
}
