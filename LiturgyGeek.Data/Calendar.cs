﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    [PrimaryKey(nameof(CalendarId))]
    [Index(nameof(CalendarCode), IsUnique = true)]
    public class Calendar
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CalendarId { get; set; }
        [JsonIgnore]
        public ICollection<CalendarItem> CalendarItems { get; set; } = null!;
        public ICollection<ChurchRule> ChurchRules { get; set; } = null!;
        [JsonIgnore]
        public CalendarDefinition? CalendarDefinition { get; set; }

        [StringLength(20)]
        public required string TraditionCode { get; set; }

        [StringLength(10)]
        public required string CalendarCode { get; set; }
    }
}
