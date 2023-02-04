using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Engine
{
    public static class DataOccasionExtensions
    {
        private static readonly char[] lBrace = { '{' };
        private static readonly char[] rBrace = { '}' };

        public static string FormatDefaultName(this Data.Occasion occasion, DateTime date)
        {
            var result = new StringBuilder();

            var parts = occasion.DefaultName.Split(lBrace, 2);

            while (parts.Length > 1)
            {
                result.Append(parts[0]);

                if (parts[1].StartsWith('{'))
                {
                    result.Append("{{");
                    parts = parts[1].Substring(1).Split(lBrace, 2);
                }
                else
                {
                    parts = parts[1].Split(rBrace, 2);
                    if (parts.Length == 1)
                        result.Append('{');
                    else
                    {
                        switch (parts[0].Trim())
                        {
                            case "dayOfWeek":
                                result.Append(date.DayOfWeek.ToString());
                                break;

                            default:
                                result.Append('{');
                                result.Append(parts[0]);
                                result.Append('}');
                                break;
                        }
                        parts = parts[1].Split(lBrace, 2);
                    }
                }
            }

            result.Append(parts[0]);

            return result.ToString();
        }
    }
}
