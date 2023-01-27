using LiturgyGeek.Common.EFCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiturgyGeek.Data
{
    [PrimaryKey(nameof(CalendarId), nameof(Date), nameof(DisplayOrder))]
    public class CalendarItem
    {
        public int CalendarId { get; set; }
        public Calendar? Calendar { get; set; }

        [DateColumn]
        public required DateTime Date { get; set; }

        public required int DisplayOrder { get; set; }

        public int? OccasionId { get; set; }
        public Occasion? Occasion { get; set; }

        [DateColumn]
        public DateTime? TransferredFrom { get; set; }

        public int? CalendarRuleId { get; set; }
        public ChurchRule? CalendarRule { get; set; }

        [JsonArrayColumn<string>]
        public required List<string> Class { get; set; } = new();
    }
}
