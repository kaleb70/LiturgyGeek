using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Data
{
    [PrimaryKey(nameof(CalendarId))]
    public class Calendar
    {
        [StringLength(10)]
        public required string CalendarId { get; set; }
        public required ICollection<CalendarItem> CalendarItems { get; set; }
    }
}
