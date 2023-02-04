using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LiturgyGeek.Data
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<LiturgyGeekContext>
    {
        private readonly IConfiguration configuration;
        private readonly IHostEnvironment hostEnvironment;

        public static void Configure(HostBuilderContext context, DbContextOptionsBuilder optionsBuilder)
            => Configure(context.Configuration, context.HostingEnvironment, optionsBuilder);

        public static void Configure(
            IConfiguration configuration,
            IHostEnvironment hostEnvironment,
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString(hostEnvironment.EnvironmentName));
        }

        public DesignTimeContextFactory(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            this.configuration = configuration;
            this.hostEnvironment = hostEnvironment;
        }

        public LiturgyGeekContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LiturgyGeekContext>();
            Configure(configuration, hostEnvironment, optionsBuilder);

            return new LiturgyGeekContext(optionsBuilder.Options);
        }
    }
}
