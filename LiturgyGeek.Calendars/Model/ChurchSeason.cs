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
    public class ChurchSeason : ICloneable<ChurchSeason>
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
    }
}
