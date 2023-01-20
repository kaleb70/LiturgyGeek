using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiturgyGeek.Data
{
    [PrimaryKey(nameof(OccasionId))]
    public class Occasion
    {
        [StringLength(20)]
        public required string OccasionId { get; set; }
        public required ICollection<CalendarItem> CalendarItems { get; set; }

        public required string DefaultName { get; set; }
    }
}
