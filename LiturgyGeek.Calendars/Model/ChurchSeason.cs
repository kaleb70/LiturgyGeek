﻿using LiturgyGeek.Calendars.Dates;
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
    public sealed class ChurchSeason : ICloneable<ChurchSeason>, IEquatable<ChurchSeason>
    {
        public required ChurchDate StartDate { get; set; }

        public required ChurchDate EndDate { get; set; }

        public bool IsDefault { get; set; }

        public HashSet<string> CommonRules { get; set; } = new HashSet<string>();

        public Dictionary<string, ChurchRuleCriteria[]> RuleCriteria { get; set; } = new Dictionary<string, ChurchRuleCriteria[]>();

        public ChurchSeason Clone()
        {
            return new ChurchSeason()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                IsDefault = IsDefault,
                CommonRules = new HashSet<string>(CommonRules),
                RuleCriteria = new(RuleCriteria),
            };
        }

        object ICloneable.Clone() => Clone();

        public bool Equals(ChurchSeason? other)
        {
            return this == other
                    || (other != null
                        && StartDate == other.StartDate
                        && EndDate == other.EndDate
                        && IsDefault == other.IsDefault
                        && CommonRules.SequenceEqual(other.CommonRules)
                        && RuleCriteria.SequenceEqual(other.RuleCriteria));
        }

        public override bool Equals(object? obj) => Equals(obj as ChurchSeason);

        public override int GetHashCode()
            => HashCode.Combine(StartDate, EndDate, IsDefault, CommonRules, RuleCriteria);
    }
}