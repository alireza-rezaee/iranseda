using Rezaee.Iranseda.JsonConverters;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rezaee.Iranseda
{
    public abstract class BaseCatalogue<TValue>
    {
        /// <summary>
        /// When the last changes were applied.
        /// </summary>
        [JsonConverter(typeof(DateTimeLocalJsonConverter))]
        public abstract DateTime LastModified { get; set; }

        /// <inheritdoc cref="JsonSerializer.Serialize{TValue}(TValue, JsonSerializerOptions?)"/>
        public static string Serialize(TValue value, JsonSerializerOptions? options = null)
            => JsonSerializer.Serialize(value, options);

        /// <inheritdoc cref="JsonSerializer.Deserialize{TValue}(string, JsonSerializerOptions?)"/>
        public static TValue Deserialize(string json, JsonSerializerOptions? options = null)
            => JsonSerializer.Deserialize<TValue>(json, options)!;

        /// <summary>
        /// Places the same <typeparamref name="TValue"/> of this object in a different reference.
        /// </summary>
        /// <param name="json"></param>
        /// <returns>A new <typeparamref name="TValue"/> object with same value.</returns>
        protected TValue Clone()
            => Deserialize(Serialize((TValue)Convert.ChangeType(this, typeof(TValue))))!;
    }
}
