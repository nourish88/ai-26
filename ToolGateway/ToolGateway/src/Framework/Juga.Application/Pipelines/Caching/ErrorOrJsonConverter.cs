using ErrorOr;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ErrorOrConverter<TValue> : JsonConverter<ErrorOr<TValue>>
{
    public override ErrorOr<TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        bool isError = false;
        List<Error> errors = null;
        TValue value = default;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (isError)
                {
                    return ErrorOr<TValue>.From(errors);
                }

                return (ErrorOr<TValue>)Activator.CreateInstance(
                    typeof(ErrorOr<TValue>),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null,
                    [value], new CultureInfo("TR-tr"));
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                string propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "IsError":
                        isError = reader.GetBoolean();
                        break;
                    case "Errors":
                        errors = JsonSerializer.Deserialize<List<Error>>(ref reader, options);
                        break;
                    case "Value":
                        value = JsonSerializer.Deserialize<TValue>(ref reader, options);
                        break;
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, ErrorOr<TValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteBoolean("IsError", value.IsError);
        if (value.IsError)
        {
            writer.WritePropertyName("Errors");
            JsonSerializer.Serialize(writer, value.Errors, options);
        }
        else
        {
            writer.WritePropertyName("Value");
            JsonSerializer.Serialize(writer, value.Value, options);
        }
        writer.WriteEndObject();
    }
}

// Usage


// Use this options object when serializing