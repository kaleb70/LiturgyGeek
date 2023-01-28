using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Data
{
    [PrimaryKey(nameof(CalendarId))]
    public class CalendarDefinition
    {
        public int CalendarId { get; set; }
        public Calendar? Calendar { get; set; }

        public required string Definition { get; set; }
    }
}
