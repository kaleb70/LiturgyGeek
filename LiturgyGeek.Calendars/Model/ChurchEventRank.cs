using LiturgyGeek.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Model
{
    public sealed class ChurchEventRank : ICloneable<ChurchEventRank>, IEquatable<ChurchEventRank>
    {
        public int Precedence { get; set; }

        public HashSet<string> Flags { get; set; } = new HashSet<string>();

        public ChurchEventRank Clone()
        {
            return new ChurchEventRank()
            {
                Precedence = Precedence,
                Flags = new HashSet<string>(Flags),
            };
        }

        object ICloneable.Clone() => Clone();

        public bool Equals(ChurchEventRank? other)
        {
            return other == this
                    || (other != null
                        && Precedence == other.Precedence
                        && Flags.SequenceEqual(other.Flags));
        }

        public override bool Equals(object? obj) => Equals(obj as ChurchEventRank);

        public override int GetHashCode() => HashCode.Combine(Precedence, Flags);
    }
}
