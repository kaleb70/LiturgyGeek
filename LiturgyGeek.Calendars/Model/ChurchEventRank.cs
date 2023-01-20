using LiturgyGeek.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Model
{
    public class ChurchEventRank : ICloneable<ChurchEventRank>
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
    }
}
