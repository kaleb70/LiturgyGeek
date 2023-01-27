using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Common;
using LiturgyGeek.Common.Collections;
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

        /// <remarks>
        /// This criterion is evaluated according to the flags of all events on a given day.
        /// </remarks>
        public IReadOnlyList<string> IncludeFlags { get; private init; }

        public IReadOnlyList<ChurchDate> IncludeDates { get; private init; }

        /// <remarks>
        /// This criterion is evaluated according to highest-ranking event on a given day,
        /// as determined by <see cref="ChurchEventRank.Precedence"/>.
        /// </remarks>
        public IReadOnlyList<string> IncludeRanks { get; private init; }

        /// <remarks>
        /// This criterion is evaluated according to the flags of all events on a given day.
        /// </remarks>
        public IReadOnlyList<string> ExcludeFlags { get; private init; }

        public IReadOnlyList<ChurchDate> ExcludeDates { get; private init; }

        private readonly int hashCode;

        [JsonConstructor]
        public ChurchRuleCriteria(string ruleCode,
                                    ChurchDate? startDate,
                                    ChurchDate? endDate,
                                    IReadOnlyList<string>? includeFlags,
                                    IReadOnlyList<ChurchDate>? includeDates,
                                    IReadOnlyList<string>? includeRanks,
                                    IReadOnlyList<string>? excludeFlags,
                                    IReadOnlyList<ChurchDate>? excludeDates)
        {
            RuleCode = ruleCode;
            StartDate = startDate;
            EndDate = endDate;
            IncludeFlags = includeFlags ?? ReadOnlyListEx<string>.Empty;
            IncludeDates = includeDates ?? ReadOnlyListEx<ChurchDate>.Empty;
            IncludeRanks = includeRanks ?? ReadOnlyListEx<string>.Empty;
            ExcludeFlags = excludeFlags ?? ReadOnlyListEx<string>.Empty;
            ExcludeDates = excludeDates ?? ReadOnlyListEx<ChurchDate>.Empty;

            var result = new HashCode();
            result.Add(RuleCode);
            result.Add(StartDate);
            result.Add(EndDate);
            result.AddList(IncludeFlags);
            result.AddList(IncludeDates);
            result.AddList(IncludeRanks);
            result.AddList(ExcludeFlags);
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
                        && IncludeFlags.SequenceEqual(other.IncludeFlags)
                        && IncludeDates.SequenceEqual(other.IncludeDates)
                        && IncludeRanks.SequenceEqual(other.IncludeRanks)
                        && ExcludeFlags.SequenceEqual(other.ExcludeFlags)
                        && ExcludeDates.SequenceEqual(other.ExcludeDates));
        }

        public override int GetHashCode() => hashCode;
    }
}
