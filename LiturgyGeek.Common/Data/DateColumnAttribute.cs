using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class DateColumnAttribute : ColumnConfigurationAttribute
    {
        public override void Configure(PropertyBuilder propertyBuilder)
        {
            propertyBuilder.HasColumnType("date");
        }
    }
}
