
using System.Text.Json;
using System.Text.Json.Serialization;

public class PrivateSetterJsonConverter : JsonConverter<object>
{
    public override bool CanConvert(Type typeToConvert)
    {
        // This converter can handle any type
        return true;
    }

    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var obj = Activator.CreateInstance(typeToConvert);

        foreach (var property in typeToConvert.GetProperties())
        {
            try
            {
                if (!property.CanWrite)
                    continue;

                var value = document.RootElement.GetProperty(property.Name).GetRawText();
                property.SetValue(obj, JsonSerializer.Deserialize(value, property.PropertyType, options));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
              
            }
            
        }

        return obj;
    }

    public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        if (value == null)
            return;

        writer.WriteStartObject();

        foreach (var property in value.GetType().GetProperties())
        {
            try
            {
                if (!property.CanWrite)
                    continue;

                writer.WritePropertyName(property.Name);
                JsonSerializer.Serialize(writer, property.GetValue(value), options);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }
           
        }

        writer.WriteEndObject();
    }
}