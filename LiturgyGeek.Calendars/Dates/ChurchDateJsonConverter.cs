using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LiturgyGeek.Calendars.Dates
{
    public class ChurchDateJsonConverter : JsonConverter<ChurchDate>
    {
        public override ChurchDate? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => ChurchDate.Parse(reader.GetString()!, CultureInfo.InvariantCulture);

        public override void Write(Utf8JsonWriter writer, ChurchDate value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}
