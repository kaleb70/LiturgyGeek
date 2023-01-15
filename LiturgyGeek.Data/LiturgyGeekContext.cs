using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Data
{
    public class LiturgyGeekContext : DbContext
    {
        public DbSet<Calendar.Occasion> Occasions { get; set; }

        public LiturgyGeekContext() : base()
        {
        }

        public LiturgyGeekContext(DbContextOptions<LiturgyGeekContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LiturgyGeekContext).Assembly);
        }
    }
}
