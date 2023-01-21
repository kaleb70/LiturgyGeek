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
    public sealed class ChurchCalendar : ICloneable<ChurchCalendar>, IEquatable<ChurchCalendar>
    {
        public required string Name { get; set; }

        public required string TraditionId { get; set; }

        public required string CalendarId { get; set; }

        public CalendarReckoning SolarReckoning { get; set; }

        public CalendarReckoning PaschalReckoning { get; set; }

        public Dictionary<string, ChurchRuleGroup> RuleGroups { get; set; } = new Dictionary<string, ChurchRuleGroup>();

        public Dictionary<string, ChurchEventRank> EventRanks { get; set; } = new Dictionary<string, ChurchEventRank>();

        public required string DefaultEventRank { get; set; }

        public Dictionary<string, Dictionary<string, ChurchRuleCriteria[]>> CommonRules { get; set; }
            = new Dictionary<string, Dictionary<string, ChurchRuleCriteria[]>>();

        public Dictionary<string, ChurchSeason> Seasons { get; set; } = new Dictionary<string, ChurchSeason>();

        public List<ChurchEvent> Events { get; set; } = new List<ChurchEvent>();

        public ChurchCalendar Clone()
        {
            var result = new ChurchCalendar()
            {
                Name = Name,
                TraditionId = TraditionId,
                CalendarId = CalendarId,
                SolarReckoning = SolarReckoning,
                PaschalReckoning = PaschalReckoning,
                RuleGroups = new(RuleGroups.WithValues(e => e.Value.Clone())),
                EventRanks = new(EventRanks.WithValues(e => e.Value.Clone())),
                DefaultEventRank = DefaultEventRank,
                CommonRules = new(CommonRules.WithValues(e => new(e.Value))),
                Seasons = new(Seasons.WithValues(e => e.Value.Clone())),
                Events = new(Events.Select(e => e.Clone())),
            };
            return result;
        }

        object ICloneable.Clone() => Clone();

        public bool Equals(ChurchCalendar? other)
        {
            return other == this
                    || (other != null
                        && Name == other.Name
                        && TraditionId == other.TraditionId
                        && CalendarId == other.CalendarId
                        && SolarReckoning == other.SolarReckoning
                        && PaschalReckoning == other.PaschalReckoning
                        && RuleGroups.SequenceEqual(other.RuleGroups)
                        && EventRanks.SequenceEqual(other.EventRanks)
                        && DefaultEventRank == other.DefaultEventRank
                        && CommonRules.SequenceEqual(other.CommonRules)
                        && Seasons.SequenceEqual(other.Seasons)
                        && Events.SequenceEqual(other.Events));
        }

        public override bool Equals(object? obj) => Equals(obj as ChurchCalendar);

        public override int GetHashCode()
        {
            var result = new HashCode();
            result.Add(Name);
            result.Add(TraditionId);
            result.Add(CalendarId);
            result.Add(SolarReckoning);
            result.Add(PaschalReckoning);
            result.Add(RuleGroups);
            result.Add(EventRanks);
            result.Add(DefaultEventRank);
            result.Add(CommonRules);
            result.Add(Seasons);
            result.Add(Events);
            return result.ToHashCode();
        }
    }
}
