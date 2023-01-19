using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.EFCore
{
    public class DbContextEx : DbContext
    {
        public DbContextEx()
        {
        }

        public DbContextEx(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var propertyInfo in entityType.ClrType.GetProperties())
                {
                    var attributes = propertyInfo.GetCustomAttributes<ColumnConfigurationAttribute>().ToArray();
                    if (attributes.Length > 1)
                        throw new InvalidOperationException($"Property {propertyInfo.Name} on {entityType.Name}" +
                                                            $" has mutually exclusive attributes" +
                                                            $" {string.Join(" and ", attributes.Select(a => a.GetType().Name))}");

                    if (attributes.Length > 0)
                    {
                        var propertyBuilder = modelBuilder.Entity(entityType.ClrType).Property(propertyInfo.Name);
                        attributes[0].Configure(propertyBuilder);
                    }
                }
            }
        }
    }
}
