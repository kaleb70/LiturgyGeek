using LiturgyGeek.Framework.Calendars;
using LiturgyGeek.Framework.Core;
using System.Text.Json;

namespace LiturgyGeek.Api.Data
{
    public class ChurchCalendarProvider : IChurchCalendarProvider
    {
        private JsonSerializerOptions jsonSerializerOptions = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public ChurchCalendar GetCalendar(string calendarKey)
        {
            return Deserialize<ChurchCalendar>($@"Data\{calendarKey}.json")
                    ?? throw new KeyNotFoundException($"Calendar not found or invalid: {calendarKey}");
        }

        public ChurchCommon GetCommon()
        {
            return Deserialize<ChurchCommon>(@"Data\ChurchCommon.json")
                    ?? throw new FileNotFoundException("Missing or invalid ChurchCommon.json");
        }

        private T? Deserialize<T>(string path, JsonSerializerOptions? options = null)
        {
            using (var stream = File.OpenRead(path))
            {
                return JsonSerializer.Deserialize<T>(stream, options ?? new()
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    PropertyNamingPolicy = JsonNamingPolicyEx.CamelCaseEx,
                });
            }
        }
    }
}
