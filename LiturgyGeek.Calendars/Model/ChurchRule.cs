using LiturgyGeek.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Model
{
    public class ChurchRule : ICloneable<ChurchRule>
    {
        public required string Summary { get; set; }

        public string? Elaboration { get; set; }

        public HashSet<string> Flags { get; set; } = new HashSet<string>();

        public HashSet<string> RemoveFlags { get; set; } = new HashSet<string>();

        public ChurchRule Clone()
        {
            return new ChurchRule()
            {
                Summary = Summary,
                Elaboration = Elaboration,
                Flags = new(Flags),
                RemoveFlags = new(RemoveFlags),
            };
        }

        object ICloneable.Clone() => Clone();
    }
}
