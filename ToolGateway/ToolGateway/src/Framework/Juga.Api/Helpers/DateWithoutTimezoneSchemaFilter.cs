
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Juga.Api.Helpers;

public class DateWithoutTimezoneSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(DateTime) || context.Type == typeof(DateTime?))
        {
            schema.Format = "date-no-tz";
            schema.Example = new OpenApiString(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }
}