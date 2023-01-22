using LiturgyGeek.Calendars.Dates;
using LiturgyGeek.Common;
using LiturgyGeek.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Model
{
    public sealed class ChurchEvent : ICloneable<ChurchEvent>, IEquatable<ChurchEvent>
    {
        public required string OccasionCode { get; set; }

        public List<ChurchDate> Dates { get; set; } = new List<ChurchDate>();

        public string? Name { get; set; }

        public string? EventRankCode { get; set; }

        public HashSet<string> Flags { get; set; } = new HashSet<string>();

        public HashSet<string> CommonRules { get; set; } = new HashSet<string>();

        public Dictionary<string, ChurchRuleCriteria[]> RuleCriteria { get; set; } = new Dictionary<string, ChurchRuleCriteria[]>();

        public List<string> AttachedSeasons { get; set; } = new List<string>();

        public List<string> AttachedEvents { get; set; } = new List<string>();

        public ChurchEvent Clone()
        {
            return new ChurchEvent()
            {
                OccasionCode = OccasionCode,
                Dates = new(Dates),
                Name = Name,
                EventRankCode = EventRankCode,
                Flags = new(Flags),
                CommonRules = new(CommonRules),
                RuleCriteria = new(RuleCriteria),
                AttachedSeasons = new(AttachedSeasons),
                AttachedEvents = new(AttachedEvents),
            };
        }

        object ICloneable.Clone() => Clone();

        public bool Equals(ChurchEvent? other)
        {
            return (other == this)
                    || (other != null
                        && OccasionCode == other.OccasionCode
                        && Dates.SequenceEqual(other.Dates)
                        && Name == other.Name
                        && EventRankCode == other.EventRankCode
                        && Flags.SetEquals(other.Flags)
                        && CommonRules.SetEquals(other.CommonRules)
                        && RuleCriteria.DictionaryEquals(other.RuleCriteria)
                        && AttachedSeasons.SequenceEqual(other.AttachedSeasons)
                        && AttachedEvents.SequenceEqual(other.AttachedEvents));
        }

        public override bool Equals(object? obj) => Equals(obj as ChurchEvent);

        public override int GetHashCode()
        {
            var result = new HashCode();
            result.Add(OccasionCode);
            result.AddList(Dates);
            result.Add(Name);
            result.Add(EventRankCode);
            result.AddSet(Flags);
            result.AddSet(CommonRules);
            result.AddDictionary(RuleCriteria);
            result.AddList(AttachedSeasons);
            result.AddList(AttachedEvents);
            return result.ToHashCode();
        }
    }
}
