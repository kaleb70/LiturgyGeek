using LiturgyGeek.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Model
{
    public sealed class ChurchRule : ICloneable<ChurchRule>, IEquatable<ChurchRule>
    {
        public required string Summary { get; set; }

        public string? Elaboration { get; set; }

        public HashSet<string> RuleFlags { get; set; } = new HashSet<string>();

        public HashSet<string> EventFlags { get; set; } = new HashSet<string>();

        public ChurchRule Clone()
        {
            return new ChurchRule()
            {
                Summary = Summary,
                Elaboration = Elaboration,
                RuleFlags = new(RuleFlags),
            };
        }

        object ICloneable.Clone() => Clone();

        public bool Equals(ChurchRule? other)
        {
            return this == other
                    || (other != null
                        && Summary == other.Summary
                        && Elaboration == other.Elaboration
                        && RuleFlags.SetEquals(other.RuleFlags));
        }

        public override bool Equals(object? obj) => Equals(obj as ChurchRule);

        public override int GetHashCode()
        {
            var result = new HashCode();
            result.Add(Summary);
            result.Add(Elaboration);
            result.AddSet(RuleFlags);
            return result.ToHashCode();
        }
    }
}
