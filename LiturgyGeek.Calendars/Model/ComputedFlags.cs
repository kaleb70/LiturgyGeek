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
    public sealed class ComputedFlags : AbstractCriteria<ComputedFlags>
    {
        public IReadOnlyList<string> AddFlags { get; private init; }

        public IReadOnlyList<string> RemoveFlags { get; private init; }

        [JsonConstructor]
        public ComputedFlags(ChurchDate? startDate,
                            ChurchDate? endDate,
                            IReadOnlyList<string>? includeFlags,
                            IReadOnlyList<ChurchDate>? includeDates,
                            IReadOnlyList<string>? includeRanks,
                            IReadOnlyList<string>? excludeFlags,
                            IReadOnlyList<ChurchDate>? excludeDates,
                            IReadOnlyList<string> addFlags,
                            IReadOnlyList<string> removeFlags)
            : base(startDate, endDate, includeFlags, includeDates, includeRanks, excludeFlags, excludeDates)
        {
            AddFlags = addFlags ?? ReadOnlyListEx<string>.Empty;
            RemoveFlags = removeFlags ?? ReadOnlyListEx<string>.Empty;
        }

        protected override HashCode ComputeHashCode()
        {
            var result = base.ComputeHashCode();
            result.AddList(AddFlags);
            result.AddList(RemoveFlags);
            return result;
        }

        public override bool Equals(ComputedFlags? other)
        {
            return base.Equals(other)
                    && AddFlags.SequenceEqual(other.AddFlags)
                    && RemoveFlags.SequenceEqual(other.RemoveFlags);
        }
    }
}
