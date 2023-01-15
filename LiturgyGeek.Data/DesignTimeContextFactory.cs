using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Data
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<LiturgyGeekContext>
    {
        public LiturgyGeekContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LiturgyGeekContext>();
            optionsBuilder.UseSqlServer("Server=dawn-treader-sql;Database=liturgygeek3_dev;User Id=sa;Password=devpw;TrustServerCertificate=True;");

            return new LiturgyGeekContext(optionsBuilder.Options);
        }
    }
}
