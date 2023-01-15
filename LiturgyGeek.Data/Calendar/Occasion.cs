using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiturgyGeek.Data.Calendar
{
    [Table("Occasions", Schema = "calendar")]
    [PrimaryKey(nameof(OccasionId))]
    [Index(nameof(OccasionCode), IsUnique = true)]
    public class Occasion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required long OccasionId { get; set; }

        [StringLength(30)]
        public required string OccasionCode { get; set; }

        public required string DefaultName { get; set; }
    }
}
