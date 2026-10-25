using System.Text.Json;

namespace DrumMachine.Demo.Documents;

/// <summary>
///  Checks the two small, explicit JSON schemas without permissive enum or polymorphic deserialization.
/// </summary>
internal static class StrictJson
{
    /// <summary>
    ///  Parses bounded UTF-8 content, accepting an editor's optional BOM but no comments or trailing commas.
    /// </summary>
    internal static JsonDocument Parse(ReadOnlyMemory<byte> utf8)
    {
        if (utf8.Span.StartsWith(new byte[] { 0xEF, 0xBB, 0xBF }))
        {
            utf8 = utf8[3..];
        }

        return JsonDocument.Parse(utf8, new JsonDocumentOptions { MaxDepth = 16 });
    }

    /// <summary>
    ///  Requires precisely the named properties, rejecting duplicate, missing, and unknown fields.
    /// </summary>
    internal static void RequireProperties(JsonElement element, params string[] names)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException("A JSON object was expected.");
        }

        int seen = 0;
        foreach (JsonProperty property in element.EnumerateObject())
        {
            int index = Array.IndexOf(names, property.Name);
            if (index < 0)
            {
                throw new InvalidDataException($"Unsupported field '{property.Name}'.");
            }

            int bit = 1 << index;
            if ((seen & bit) != 0)
            {
                throw new InvalidDataException($"Duplicate field '{property.Name}'.");
            }

            seen |= bit;
        }

        for (int index = 0; index < names.Length; index++)
        {
            if ((seen & (1 << index)) == 0)
            {
                throw new InvalidDataException($"Required field '{names[index]}' is missing.");
            }
        }
    }

    /// <summary>
    ///  Reads a required string without coercing numbers or null values.
    /// </summary>
    internal static string String(JsonElement value, string name)
    {
        if (value.ValueKind != JsonValueKind.String)
        {
            throw new InvalidDataException($"'{name}' must be a string.");
        }

        return value.GetString()!;
    }

    /// <summary>
    ///  Reads a required integral value within an explicit inclusive range.
    /// </summary>
    internal static int Integer(JsonElement value, string name, int minimum, int maximum)
    {
        if (value.ValueKind != JsonValueKind.Number ||
            !value.TryGetInt32(out int result) ||
            result < minimum || result > maximum)
        {
            throw new InvalidDataException($"'{name}' must be an integer from {minimum} through {maximum}.");
        }

        return result;
    }

    /// <summary>
    ///  Reads a real JSON Boolean rather than interpreting numeric or textual substitutes.
    /// </summary>
    internal static bool Boolean(JsonElement value, string name)
    {
        if (value.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
        {
            throw new InvalidDataException($"'{name}' must be true or false.");
        }

        return value.GetBoolean();
    }

    /// <summary>
    ///  Requires an array before inspecting its elements.
    /// </summary>
    internal static void RequireArray(JsonElement value, string name)
    {
        if (value.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException($"'{name}' must be an array.");
        }
    }
}
