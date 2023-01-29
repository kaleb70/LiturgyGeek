using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Common.Collections;
using LiturgyGeek.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Model
{
    public abstract class AbstractCriteria<T> : IEquatable<T> where T : AbstractCriteria<T>
    {
        public ChurchDate? StartDate { get; private init; }

        public ChurchDate? EndDate { get; private init; }

        /// <remarks>
        /// When applied to a day, this criterion is evaluated according to the flags of all events on that day.
        /// </remarks>
        public IReadOnlyList<string> IncludeFlags { get; private init; }

        public IReadOnlyList<ChurchDate> IncludeDates { get; private init; }

        /// <remarks>
        /// When applied to a day, this criterion is evaluated according to highest-ranking event on that day,
        /// as determined by <see cref="ChurchEventRank.Precedence"/>.
        /// </remarks>
        public IReadOnlyList<string> IncludeRanks { get; private init; }

        /// <remarks>
        /// When applied to a day, this criterion is evaluated according to the flags of all events on that day.
        /// </remarks>
        public IReadOnlyList<string> ExcludeFlags { get; private init; }

        public IReadOnlyList<ChurchDate> ExcludeDates { get; private init; }

        private int? hashCode;

        public AbstractCriteria(ChurchDate? startDate,
                                ChurchDate? endDate,
                                IReadOnlyList<string>? includeFlags,
                                IReadOnlyList<ChurchDate>? includeDates,
                                IReadOnlyList<string>? includeRanks,
                                IReadOnlyList<string>? excludeFlags,
                                IReadOnlyList<ChurchDate>? excludeDates)
        {
            StartDate = startDate;
            EndDate = endDate;
            IncludeFlags = includeFlags ?? ReadOnlyListEx<string>.Empty;
            IncludeDates = includeDates ?? ReadOnlyListEx<ChurchDate>.Empty;
            IncludeRanks = includeRanks ?? ReadOnlyListEx<string>.Empty;
            ExcludeFlags = excludeFlags ?? ReadOnlyListEx<string>.Empty;
            ExcludeDates = excludeDates ?? ReadOnlyListEx<ChurchDate>.Empty;
        }

        protected virtual HashCode ComputeHashCode()
        {
            var result = new HashCode();
            result.Add(StartDate);
            result.Add(EndDate);
            result.AddList(IncludeFlags);
            result.AddList(IncludeDates);
            result.AddList(IncludeRanks);
            result.AddList(ExcludeFlags);
            result.AddList(ExcludeDates);
            return result;
        }

        public virtual bool Equals(T? other)
        {
            return this == other
                    || (other != null
                        && StartDate == other.StartDate
                        && EndDate == other.EndDate
                        && IncludeFlags.SequenceEqual(other.IncludeFlags)
                        && IncludeDates.SequenceEqual(other.IncludeDates)
                        && IncludeRanks.SequenceEqual(other.IncludeRanks)
                        && ExcludeFlags.SequenceEqual(other.ExcludeFlags)
                        && ExcludeDates.SequenceEqual(other.ExcludeDates));
        }

        public sealed override bool Equals(object? obj)
        {
            return obj?.GetType() == typeof(T) && Equals((T)obj);
        }

        public sealed override int GetHashCode() => hashCode ??= ComputeHashCode().ToHashCode();
    }
}
