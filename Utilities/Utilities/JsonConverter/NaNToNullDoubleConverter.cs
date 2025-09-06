using Newtonsoft.Json;
using System;

namespace Utilities.JsonConverter;

public class NaNToNullDoubleConverter : JsonConverter<double?>
{
    public override double? ReadJson(JsonReader reader, Type objectType, double? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Float && double.IsNaN(Convert.ToDouble(reader.Value)))
        {
            return null;
        }
        return Convert.ToDouble(reader.Value);
    }

    public override void WriteJson(JsonWriter writer, double? value, JsonSerializer serializer)
    {
        if (value.HasValue && double.IsNaN(value.Value))
        {
            writer.WriteNull();
        }
        else
        {
            writer.WriteValue(value);
        }
    }
}
