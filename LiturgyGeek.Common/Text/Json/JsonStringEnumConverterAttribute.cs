using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Common.Text.Json
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface, AllowMultiple = false)]
    public class JsonStringEnumConverterAttribute : JsonConverterAttribute
    {
        private static readonly JsonNamingPolicy?[] namingPolicies = new[]
        {
            default,
            JsonNamingPolicy.CamelCase,
        };

        private readonly JsonKnownNamingPolicy namingPolicy;
        private readonly bool allowIntegerValues;
        public JsonStringEnumConverterAttribute(JsonKnownNamingPolicy namingPolicy = JsonKnownNamingPolicy.Unspecified, bool allowIntegerValues = true)
        {
            this.namingPolicy = namingPolicy;
            this.allowIntegerValues = allowIntegerValues;
        }

        public override JsonConverter CreateConverter(Type typeToConvert) => new JsonStringEnumConverter(namingPolicies[(int)namingPolicy], allowIntegerValues);
    }
}
