using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Data
{
    public class ChurchRule
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CalendarRuleId { get; set; }
        public ICollection<CalendarItem> CalendarItems { get; set; } = null!;

        public int CalendarId { get; set; }
        public Calendar? Calendar { get; set; }

        [StringLength(20)]
        public required string CalendarRuleCode { get; set; }
    }
}
