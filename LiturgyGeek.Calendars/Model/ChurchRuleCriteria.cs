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
    public sealed class ChurchRuleCriteria : AbstractCriteria<ChurchRuleCriteria>
    {
        public string RuleCode { get; private init; }

        [JsonConstructor]
        public ChurchRuleCriteria(ChurchDate? startDate,
                                    ChurchDate? endDate,
                                    IReadOnlyList<string>? includeFlags,
                                    IReadOnlyList<ChurchDate>? includeDates,
                                    IReadOnlyList<string>? includeRanks,
                                    IReadOnlyList<string>? excludeFlags,
                                    IReadOnlyList<ChurchDate>? excludeDates,
                                    string ruleCode)
            : base(startDate, endDate,includeFlags, includeDates, includeRanks, excludeFlags, excludeDates)
        {
            RuleCode = ruleCode;
        }

        protected override HashCode ComputeHashCode()
        {
            var result = base.ComputeHashCode();
            result.Add(RuleCode);
            return result;
        }

        public override bool Equals(ChurchRuleCriteria? other)
        {
            return base.Equals(other) && RuleCode == other.RuleCode;
        }
    }
}
