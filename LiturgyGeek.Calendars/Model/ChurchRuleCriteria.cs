using LiturgyGeek.Calendars.Dates;
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
        public string RuleId { get; private init; }

        public ChurchDate? StartDate { get; private init; }

        public ChurchDate? EndDate { get; private init; }

        public IReadOnlyList<string> IncludeCustomFlags { get; private init; }

        public IReadOnlyList<ChurchDate> IncludeDates { get; private init; }

        public IReadOnlyList<string> IncludeRanks { get; private init; }

        public IReadOnlyList<string> ExcludeCustomFlags { get; private init; }

        public IReadOnlyList<ChurchDate> ExcludeDates { get; private init; }

        [JsonConstructor]
        public ChurchRuleCriteria(string ruleId,
                                    ChurchDate? startDate,
                                    ChurchDate? endDate,
                                    IReadOnlyList<string>? includeCustomFlags,
                                    IReadOnlyList<ChurchDate>? includeDates,
                                    IReadOnlyList<string>? includeRanks,
                                    IReadOnlyList<string>? excludeCustomFlags,
                                    IReadOnlyList<ChurchDate>? excludeDates)
        {
            RuleId = ruleId;
            StartDate = startDate;
            EndDate = endDate;
            IncludeCustomFlags = includeCustomFlags ?? new List<string>();
            IncludeDates = includeDates ?? new List<ChurchDate>();
            IncludeRanks = includeRanks ?? new List<string>();
            ExcludeCustomFlags = excludeCustomFlags ?? new List<string>();
            ExcludeDates = excludeDates ?? new List<ChurchDate>();
        }

        public bool Equals(ChurchRuleCriteria? other)
        {
            return this == other
                    || (other != null
                        && RuleId == other.RuleId
                        && StartDate == other.StartDate
                        && EndDate == other.EndDate
                        && IncludeCustomFlags.SequenceEqual(other.IncludeCustomFlags)
                        && IncludeDates.SequenceEqual(other.IncludeDates)
                        && IncludeRanks.SequenceEqual(other.IncludeRanks)
                        && ExcludeCustomFlags.SequenceEqual(other.ExcludeCustomFlags)
                        && ExcludeDates.SequenceEqual(other.ExcludeDates));
        }

        public override int GetHashCode()
            => HashCode.Combine(RuleId, StartDate, EndDate,
                                IncludeCustomFlags, IncludeDates, IncludeRanks,
                                ExcludeCustomFlags, ExcludeDates);
    }
}
