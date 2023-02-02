using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Calendars.Model;
using LiturgyGeek.Common.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Engine
{
    public class CalendarReader
    {
        private static readonly char[] whitespace = { ' ', '\t' };
        private static readonly char[] quote = { '"' };

        private const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries;

        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNamingPolicy = JsonNamingPolicyEx.CamelCaseEx,
            IgnoreReadOnlyFields = true,
        };

        public ChurchCalendar Read(Stream jsonStream, Stream? lineStream = null, bool preprocess = true)
        {
            var calendar = JsonSerializer.Deserialize<ChurchCalendar>(jsonStream, jsonOptions)!;
            if (lineStream != null)
            {
                using (var lineReader  = new StreamReader(lineStream))
                    ApplyLines(calendar, lineReader);
            }
            if (preprocess)
                calendar.Preprocess(this);
            return calendar;
        }

        public ChurchCalendar Read(string json, string? lines = null, bool preprocess = true)
        {
            var calendar = JsonSerializer.Deserialize<ChurchCalendar>(json, jsonOptions)!;
            if (lines != null)
            {
                using (var lineReader = new StringReader(lines))
                    ApplyLines(calendar, lineReader);
            }
            if (preprocess)
                calendar.Preprocess(this);
            return calendar;
        }

        public Data.Occasion[] ReadOccasions(Stream lineStream)
        {
            var result = new List<Data.Occasion>();
            using (var lineReader = new StreamReader(lineStream))
                ApplyLines(result, lineReader);
            return result.ToArray();
        }

        public Data.Occasion[] ReadOccasions(string lines)
        {
            var result = new List<Data.Occasion>();
            using (var lineReader = new StringReader(lines))
                ApplyLines(result, lineReader);
            return result.ToArray();
        }

        private void ApplyLines(ChurchCalendar calendar, TextReader lineReader)
        {
            string? line;
            while ((line = lineReader.ReadLine()?.Trim()) != null)
            {
                if (line.Length > 0 && !line.StartsWith(';'))
                    calendar.Events.Add(ParseEventLine(line));
            }
        }

        private void ApplyLines(List<Data.Occasion> occasions, TextReader lineReader)
        {
            string? line;
            while ((line = lineReader.ReadLine()?.Trim()) != null)
            {
                if (line.Length > 0 && !line.StartsWith(';'))
                    occasions.Add(ParseOccasionLine(line));
            }
        }

        public void ParseSeasonLine(string line, out string seasonCode, out ChurchSeason season)
        {
            var parts = line.Split(whitespace, 2, splitOptions);
            if (parts.Length < 2)
                throw new InvalidDataException();

            seasonCode = parts[0];

            parts = parts[1].Split(whitespace, 2, splitOptions);
            if (parts.Length < 2 || parts[0].Length < 1 || parts[0][0] != '@')
                throw new InvalidDataException();

            var dateParts = parts[0].Substring(1).Split("..", 2);
            var startDate = ChurchDate.Parse(dateParts[0]);
            var endDate = ChurchDate.Parse(dateParts[dateParts.Length - 1]);

            season = new ChurchSeason()
            {
                StartDate = startDate,
                EndDate = endDate,
            };

            if (parts.Length > 1)
            {
                do
                {
                    parts = parts[1].Split(whitespace, 2, splitOptions);

                    switch (parts[0][0])
                    {
                        case '$':
                            season.CommonRules.Add(parts[0].Substring(1));
                            break;

                        case '#':
                            season.Flags.Add(parts[0].Substring(1));
                            break;

                        default:
                            throw new InvalidDataException();
                    }

                } while (parts.Length > 1);
            }
        }

        public ChurchEvent ParseEventLine(string line)
        {
            var parts = line.Split(whitespace, 2, splitOptions);
            if (parts.Length < 2)
                throw new InvalidDataException();

            var result = new ChurchEvent()
            {
                OccasionCode = parts[0],
                Dates = new(),
            };

            do
            {
                parts = parts[1].Split(whitespace, 2, splitOptions);

                switch (parts[0][0])
                {
                    case '@':
                        result.Dates.Add(ChurchDate.Parse(parts[0].Substring(1)));
                        break;

                    case '$':
                        result.CommonRules.Add(parts[0].Substring(1));
                        break;

                    case '#':
                        result.Flags.Add(parts[0].Substring(1));
                        break;

                    case '+':
                        result.EventRankCode = parts[0].Substring(1);
                        break;

                    default:
                        throw new InvalidDataException();
                }

            } while (parts.Length > 1);

            if (result.Dates.Count == 0)
                throw new InvalidDataException();

            return result;
        }

        public Data.Occasion ParseOccasionLine(string line)
        {
            var parts = line.Split(whitespace, 2, splitOptions);
            if (parts.Length < 2)
                throw new InvalidDataException();

            var occasionCode = parts[0];

            if (parts[1].Length < 3 || parts[1][0] != '"')
                throw new InvalidDataException();

            parts = parts[1].Substring(1).Split(quote, 2);
            if (parts.Length < 2)
                throw new InvalidDataException();

            var defaultName = parts[0];
            while (parts[1].Length > 0 && parts[1][0] == '"')
            {
                defaultName += '"';
                parts = parts[1].Substring(1).Split(quote, 2);
                if (parts.Length < 2)
                    throw new InvalidDataException();
                defaultName += parts[0];
            }

            if (parts[1].Length > 0)
                throw new InvalidDataException();

            return new Data.Occasion()
            {
                OccasionCode = occasionCode,
                DefaultName = defaultName,
            };
        }
    }
}
