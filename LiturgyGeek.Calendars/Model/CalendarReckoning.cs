using LiturgyGeek.Common.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Model
{
    [JsonStringEnumConverter(JsonKnownNamingPolicy.CamelCase)]
    public enum CalendarReckoning
    {
        Julian,
        Gregorian,
        RevisedJulian,
    }
}
