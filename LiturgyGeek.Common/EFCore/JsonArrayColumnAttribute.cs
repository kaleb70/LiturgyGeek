using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.EFCore
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonArrayColumnAttribute<T> : ColumnConfigurationAttribute
        where T : notnull
    {
        public override void Configure(PropertyBuilder propertyBuilder)
        {
            propertyBuilder.HasConversion(
                new ValueConverter<List<T>, string>
                (
                    v => JsonSerializer.Serialize(v, default(JsonSerializerOptions)),
                    v => JsonSerializer.Deserialize<List<T>>(v, default(JsonSerializerOptions))!
                ),
                new ValueComparer<List<T>>
                (
                    (x, y) => x!.SequenceEqual(y!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                )
            );

        }
    }
}
