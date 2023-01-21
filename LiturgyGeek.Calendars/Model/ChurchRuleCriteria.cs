using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Model
{
    public sealed class ChurchRuleCriteria : IEquatable<ChurchRuleCriteria>
    {
        public string RuleCode { get; private init; }

        public ChurchDate? StartDate { get; private init; }

        public ChurchDate? EndDate { get; private init; }

        public IReadOnlyList<string> IncludeCustomFlags { get; private init; }

        public IReadOnlyList<ChurchDate> IncludeDates { get; private init; }

        public IReadOnlyList<string> IncludeRanks { get; private init; }

        public IReadOnlyList<string> ExcludeCustomFlags { get; private init; }

        public IReadOnlyList<ChurchDate> ExcludeDates { get; private init; }

        private readonly int hashCode;

        [JsonConstructor]
        public ChurchRuleCriteria(string ruleCode,
                                    ChurchDate? startDate,
                                    ChurchDate? endDate,
                                    IReadOnlyList<string>? includeCustomFlags,
                                    IReadOnlyList<ChurchDate>? includeDates,
                                    IReadOnlyList<string>? includeRanks,
                                    IReadOnlyList<string>? excludeCustomFlags,
                                    IReadOnlyList<ChurchDate>? excludeDates)
        {
            RuleCode = ruleCode;
            StartDate = startDate;
            EndDate = endDate;
            IncludeCustomFlags = includeCustomFlags ?? new List<string>();
            IncludeDates = includeDates ?? new List<ChurchDate>();
            IncludeRanks = includeRanks ?? new List<string>();
            ExcludeCustomFlags = excludeCustomFlags ?? new List<string>();
            ExcludeDates = excludeDates ?? new List<ChurchDate>();

            var result = new HashCode();
            result.Add(RuleCode);
            result.Add(StartDate);
            result.Add(EndDate);
            result.AddList(IncludeCustomFlags);
            result.AddList(IncludeDates);
            result.AddList(IncludeRanks);
            result.AddList(ExcludeCustomFlags);
            result.AddList(ExcludeDates);
            hashCode = result.ToHashCode();
        }

        public bool Equals(ChurchRuleCriteria? other)
        {
            return this == other
                    || (other != null
                        && RuleCode == other.RuleCode
                        && StartDate == other.StartDate
                        && EndDate == other.EndDate
                        && IncludeCustomFlags.SequenceEqual(other.IncludeCustomFlags)
                        && IncludeDates.SequenceEqual(other.IncludeDates)
                        && IncludeRanks.SequenceEqual(other.IncludeRanks)
                        && ExcludeCustomFlags.SequenceEqual(other.ExcludeCustomFlags)
                        && ExcludeDates.SequenceEqual(other.ExcludeDates));
        }

        public override int GetHashCode() => hashCode;
    }
}
