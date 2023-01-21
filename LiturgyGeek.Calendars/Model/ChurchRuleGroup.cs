using LiturgyGeek.Common;
using LiturgyGeek.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Model
{
    public sealed class ChurchRuleGroup : ICloneable<ChurchRuleGroup>, IEquatable<ChurchRuleGroup>
    {
        public HashSet<string> Flags { get; set; } = new HashSet<string>();

        public Dictionary<string, ChurchRule> Rules { get; set; } = new Dictionary<string, ChurchRule>();

        public ChurchRuleGroup Clone()
        {
            return new ChurchRuleGroup()
            {
                Flags = new(Flags),
                Rules = new(Rules.WithValues(e => e.Value.Clone())),
            };
        }

        object ICloneable.Clone() => Clone();

        public bool Equals(ChurchRuleGroup? other)
        {
            return this == other
                    || (other != null
                        && Flags.SequenceEqual(other.Flags)
                        && Rules.SequenceEqual(other.Rules));
        }

        public override bool Equals(object? obj) => Equals(obj as ChurchRuleGroup);

        public override int GetHashCode() => HashCode.Combine(Flags, Rules);
    }
}
