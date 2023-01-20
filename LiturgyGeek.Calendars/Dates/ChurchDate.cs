using LiturgyGeek.Common.Globalization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Dates
{
    [JsonConverter(typeof(ChurchDateJsonConverter))]
    public abstract class ChurchDate
    {
        private static readonly char[] slashSeparator = new[] { '/' };
        private static readonly char[] hyphenSeparator = new[] { '-' };
        private static readonly char[] semicolonSeparator = new[] { ';' };

        public static implicit operator ChurchDate(string text) => Parse(text);

        public static IEnumerable<ChurchDate> ParseCollection(string text) => ParseCollection(text, CultureInfo.InvariantCulture);

        public static IEnumerable<ChurchDate> ParseCollection(string text, CultureInfo cultureInfo)
            => text.Split(semicolonSeparator).Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => Parse(t, cultureInfo));

        public static bool TryParse(string text, [NotNullWhen(true)] out ChurchDate? result)
        {
            try
            {
                result = Parse(text);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static bool TryParse(string text, CultureInfo cultureInfo, [NotNullWhen(true)] out ChurchDate? result)
        {
            try
            {
                result = Parse(text, cultureInfo);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public static ChurchDate Parse(string text) => Parse(text, CultureInfo.InvariantCulture);

        public static ChurchDate Parse(string text, CultureInfo cultureInfo)
        {
            text = text.Trim();
            if (text.Any(c => char.IsWhiteSpace(c)))
                throw new FormatException();

            var parsed = text.Split(slashSeparator, 2);

            if (parsed.Length == 1)
            {
                return new WeeklyDate(GeneralParser.ParseDayOfWeek(parsed[0], cultureInfo));
            }
            else if (parsed[0] == "*")
            {
                parsed = parsed[1].Split(slashSeparator);

                if (parsed.Length > 2)
                    throw new FormatException();

                int day = int.Parse(parsed[0], cultureInfo);
                DayOfWeek? dayOfWeek = parsed.Length > 1 ? GeneralParser.ParseDayOfWeek(parsed[1], cultureInfo) : default(DayOfWeek?);

                return new MonthlyDate(day, dayOfWeek);
            }
            else if (parsed.Length < 2)
                throw new FormatException();

            else
            {
                var leftValue = int.Parse(parsed[0], cultureInfo);

                parsed = parsed[1].Split(slashSeparator);

                if (int.TryParse(parsed[0], out var day))
                {
                    DayOfWeek? startDayOfWeek = default;
                    DayOfWeek? endDayOfWeek = default;
                    int? window = default;

                    if (parsed.Length > 1)
                    {
                        var parsedDayOfWeek = parsed[1].Split(hyphenSeparator, 2);

                        startDayOfWeek = GeneralParser.ParseDayOfWeek(parsedDayOfWeek[0], cultureInfo);
                        if (parsedDayOfWeek.Length > 1)
                        {
                            endDayOfWeek = GeneralParser.ParseDayOfWeek(parsedDayOfWeek[1], cultureInfo);

                            if (parsed.Length > 2)
                                throw new FormatException();
                        }
                        else if (parsed.Length > 2)
                        {
                            if (parsed.Length > 3)
                                throw new FormatException();

                            window = int.Parse(parsed[2], cultureInfo);
                        }
                    }

                    return new FixedDate(leftValue, day, startDayOfWeek, endDayOfWeek, window);
                }
                else if (parsed.Length > 1)
                    throw new FormatException();

                else
                    return new MovableDate(leftValue, GeneralParser.ParseDayOfWeek(parsed[0], cultureInfo));
            }
        }

        public sealed override string ToString() => ToString(CultureInfo.InvariantCulture);

        public abstract string ToString(CultureInfo culture);

        public abstract bool IsRecurring { get; }

        public abstract bool IsMovable { get; }

        public abstract DateTime? GetInstance(ChurchCalendarSystem calendarSystem, int year, DateTime? seed = default);

        public virtual DateTime? GetInstanceFollowing(ChurchDate? priorDate, ChurchCalendarSystem calendarSystem, int year)
            => GetInstance(calendarSystem, year);

        public bool Equals(string s) => TryParse(s, out var other) && Equals(other);

        public bool Equals(string s, CultureInfo cultureInfo) => TryParse(s, cultureInfo, out var other) && Equals(other);
    }
}
