using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.EFCore
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class ColumnConfigurationAttribute : Attribute
    {
        public abstract void Configure(PropertyBuilder propertyBuilder);
    }
}
