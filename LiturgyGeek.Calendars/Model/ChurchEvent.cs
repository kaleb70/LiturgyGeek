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
        public required string OccasionId { get; set; }

        public List<ChurchDate> Dates { get; set; } = new List<ChurchDate>();

        public string? Name { get; set; }

        public string? EventRankId { get; set; }

        public HashSet<string> Flags { get; set; } = new HashSet<string>();

        public HashSet<string> CommonRules { get; set; } = new HashSet<string>();

        public Dictionary<string, ChurchRuleCriteria[]> RuleCriteria { get; set; } = new Dictionary<string, ChurchRuleCriteria[]>();

        public Dictionary<string, ChurchSeason> AttachedSeasons { get; set; } = new Dictionary<string, ChurchSeason>();

        public List<ChurchEvent> AttachedEvents { get; set; } = new List<ChurchEvent>();

        public ChurchEvent Clone()
        {
            return new ChurchEvent()
            {
                OccasionId = OccasionId,
                Dates = new(Dates),
                Name = Name,
                EventRankId = EventRankId,
                Flags = new(Flags),
                CommonRules = new(CommonRules),
                RuleCriteria = new(RuleCriteria),
                AttachedSeasons = new(AttachedSeasons.WithValues(e => e.Value.Clone())),
                AttachedEvents = new(AttachedEvents.Select(e => e.Clone())),
            };
        }

        object ICloneable.Clone() => Clone();

        public bool Equals(ChurchEvent? other)
        {
            return (other == this)
                    || (other != null
                        && OccasionId == other.OccasionId
                        && Dates.SequenceEqual(other.Dates)
                        && Name == other.Name
                        && EventRankId == other.EventRankId
                        && Flags.SequenceEqual(other.Flags)
                        && CommonRules.SequenceEqual(other.CommonRules)
                        && RuleCriteria.SequenceEqual(other.RuleCriteria)
                        && AttachedSeasons.SequenceEqual(other.AttachedSeasons)
                        && AttachedEvents.SequenceEqual(other.AttachedEvents));
        }

        public override bool Equals(object? obj) => Equals(obj as ChurchEvent);

        public override int GetHashCode()
        {
            var result = new HashCode();
            result.Add(OccasionId);
            result.Add(Dates);
            result.Add(Name);
            result.Add(EventRankId);
            result.Add(Flags);
            result.Add(CommonRules);
            result.Add(RuleCriteria);
            result.Add(AttachedSeasons);
            result.Add(AttachedEvents);
            return result.ToHashCode();
        }
    }
}
