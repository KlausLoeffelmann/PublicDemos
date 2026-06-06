using System.Text.Json;
using System.Text.Json.Serialization;
using LayoutTests.App.Models;

namespace LayoutTests.App.Services;

public static class ProbeSetSerializer
{
    private static readonly JsonSerializerOptions s_options = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new SizeJsonConverter());
        return options;
    }

    public static string Serialize(ProbeSet set) => JsonSerializer.Serialize(set, s_options);

    public static ProbeSet Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        return JsonSerializer.Deserialize<ProbeSet>(json, s_options)
            ?? throw new InvalidDataException("ProbeSet JSON was empty or malformed.");
    }

    private sealed class SizeJsonConverter : JsonConverter<Size>
    {
        public override Size Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected object for Size.");
            }

            int width = 0;
            int height = 0;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new Size(width, height);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    continue;
                }

                string name = reader.GetString() ?? string.Empty;
                reader.Read();

                if (string.Equals(name, "Width", StringComparison.OrdinalIgnoreCase))
                {
                    width = reader.GetInt32();
                }
                else if (string.Equals(name, "Height", StringComparison.OrdinalIgnoreCase))
                {
                    height = reader.GetInt32();
                }
            }

            throw new JsonException("Unexpected end of JSON while reading Size.");
        }

        public override void Write(Utf8JsonWriter writer, Size value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("Width", value.Width);
            writer.WriteNumber("Height", value.Height);
            writer.WriteEndObject();
        }
    }
}
