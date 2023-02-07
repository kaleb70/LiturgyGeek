using LiturgyGeek.Common;
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

        public static string FormatDefaultName(this Data.Occasion occasion, DateTime date, DateTime? referenceDate = null)
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
                        string field = parts[0].Trim();
                        switch (field)
                        {
                            case "dayOfWeek":
                                result.Append(date.DayOfWeek.ToString());
                                break;

                            case "week":
                                if (!referenceDate.HasValue)
                                    goto default;
                                result.Append(date.Subtract(referenceDate.Value).GetWeeks() + 1);
                                break;

                            case "weekOrdinal":
                                if (!referenceDate.HasValue)
                                    goto default;
                                result.AppendOrdinal(date.Subtract(referenceDate.Value).GetWeeks() + 1);
                                break;

                            case "day":
                                if (!referenceDate.HasValue)
                                    goto default;
                                result.Append(date.Subtract(referenceDate.Value).Days + 1);
                                break;

                            case "dayOrdinal":
                                if (!referenceDate.HasValue)
                                    goto default;
                                result.AppendOrdinal(date.Subtract(referenceDate.Value).Days + 1);
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
