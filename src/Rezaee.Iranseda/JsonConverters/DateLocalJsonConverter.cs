using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rezaee.Iranseda.JsonConverters
{
    /// <summary>
    /// A <see cref="JsonConverter"/> to convert Iran local date to Gregorian.
    /// </summary>
    public class DateLocalJsonConverter : JsonConverter<DateTime>
    {
        private readonly CultureInfo _culture;
        private const string DateFormat = "yyyy-MM-dd";

        public DateLocalJsonConverter()
            => _culture = new CultureInfo("fa-IR");

        /// <inheritdoc/>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.ParseExact(reader.GetString(), DateFormat, _culture);

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(DateFormat, _culture));
    }
}
