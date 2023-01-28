using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Data
{
    [PrimaryKey(nameof(ChurchRuleId))]
    [Index(nameof(CalendarId), nameof(RuleGroupCode), nameof(RuleCode), IsUnique = true)]
    public class ChurchRule
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChurchRuleId { get; set; }
        [JsonIgnore]
        public ICollection<CalendarItem> CalendarItems { get; set; } = null!;

        public int CalendarId { get; set; }
        [JsonIgnore]
        public Calendar? Calendar { get; set; }

        [StringLength(20)]
        public required string RuleGroupCode { get; set; }

        [StringLength(20)]
        public required string RuleCode { get; set; }

        public required string Summary { get; set; }

        public string? Elaboration { get; set; }
    }
}
