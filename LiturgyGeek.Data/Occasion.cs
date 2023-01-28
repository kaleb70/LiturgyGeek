using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LiturgyGeek.Data
{
    [PrimaryKey(nameof(OccasionId))]
    [Index(nameof(OccasionCode), IsUnique = true)]
    public class Occasion
    {
        public int OccasionId { get; set; }
        [JsonIgnore]
        public ICollection<CalendarItem> CalendarItems { get; set; } = null!;

        [StringLength(40)]
        public required string OccasionCode { get; set; }

        public required string DefaultName { get; set; }
    }
}
