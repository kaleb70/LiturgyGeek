using LiturgyGeek.Calendars.Model;
using LiturgyGeek.Common;
using LiturgyGeek.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Framework.Clcs.Model
{
    public class ChurchCalendar : ICloneable<ChurchCalendar>
    {
        public required string Name { get; set; }

        public required string TraditionCode { get; set; }

        public required string CalendarCode { get; set; }

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
                TraditionCode = TraditionCode,
                CalendarCode = CalendarCode,
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
    }
}
