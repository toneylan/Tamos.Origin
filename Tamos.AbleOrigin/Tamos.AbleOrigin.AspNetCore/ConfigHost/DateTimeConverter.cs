using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tamos.AbleOrigin.AspNetCore
{
    public class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //return reader.GetDateTime();
            return reader.GetString().ToDate();
        }
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}