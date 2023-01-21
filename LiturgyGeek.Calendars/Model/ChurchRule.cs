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

        public HashSet<string> Flags { get; set; } = new HashSet<string>();

        public ChurchRule Clone()
        {
            return new ChurchRule()
            {
                Summary = Summary,
                Elaboration = Elaboration,
                Flags = new(Flags),
            };
        }

        object ICloneable.Clone() => Clone();

        public bool Equals(ChurchRule? other)
        {
            return this == other
                    || (other != null
                        && Summary == other.Summary
                        && Elaboration == other.Elaboration
                        && Flags.SequenceEqual(other.Flags));
        }

        public override bool Equals(object? obj) => Equals(obj as ChurchRule);

        public override int GetHashCode() => HashCode.Combine(Summary, Elaboration, Flags);
    }
}
