using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonColumnAttribute<T> : ColumnConfigurationAttribute
    {
        public override void Configure(PropertyBuilder propertyBuilder)
        {
            propertyBuilder.HasConversion(new ValueConverter<T, string>
            (
                v => JsonSerializer.Serialize(v, default(JsonSerializerOptions)),
                v => JsonSerializer.Deserialize<T>(v, default(JsonSerializerOptions))!
            ));
        }
    }
}
