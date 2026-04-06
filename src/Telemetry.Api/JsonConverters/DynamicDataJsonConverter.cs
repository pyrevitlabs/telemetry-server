using System.Text.Json;
using System.Text.Json.Serialization;

namespace Telemetry.Api.JsonConverters;

internal class DynamicDataJsonConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        using JsonDocument parsed = JsonDocument.ParseValue(ref reader);
        return parsed.RootElement.GetRawText();
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}