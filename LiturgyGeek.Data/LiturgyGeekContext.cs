﻿using LiturgyGeek.Common.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Data
{
    public class LiturgyGeekContext : DbContextEx
    {
        public DbSet<Occasion> Occasions { get; set; }

        public DbSet<Calendar> Calendars { get; set; }

        public DbSet<CalendarDefinition> CalendarDefinitions { get; set; }

        public DbSet<CalendarItem> CalendarItems { get; set; }

        public DbSet<ChurchRule> ChurchRules { get; set; }

        public LiturgyGeekContext() : base()
        {
        }

        public LiturgyGeekContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
