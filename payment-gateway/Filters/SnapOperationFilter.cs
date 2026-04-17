using Microsoft.OpenApi.Models;
using payment_gateway.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace payment_gateway.Filter
{
    public class SnapOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAttribute = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<SnapHeaderAttribute>()
            .Any();

            if (!hasAttribute)
                return;

            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            var headers = new List<OpenApiParameter>
        {
            new OpenApiParameter
            {
                Name = "X-SIGNATURE",
                In = ParameterLocation.Header,
                Required = true,
                Description = "Signature for request validation",
                Schema = new OpenApiSchema { Type = "string" }
            },
            new OpenApiParameter
            {
                Name = "X-TIMESTAMP",
                In = ParameterLocation.Header,
                Required = true,
                Description = "ISO8601 timestamp (yyyy-MM-ddTHH:mm:sszzz)",
                Schema = new OpenApiSchema { Type = "string", Example = new Microsoft.OpenApi.Any.OpenApiString("2026-04-17T10:00:00+07:00") }
            },
            new OpenApiParameter
            {
                Name = "X-PARTNER-ID",
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema { Type = "string" }
            },
            new OpenApiParameter
            {
                Name = "X-EXTERNAL-ID",
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema { Type = "string" }
            },
            new OpenApiParameter
            {
                Name = "CHANNEL-ID",
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema { Type = "string" }
            }
        };

            foreach (var header in headers)
            {
                // Hindari duplicate
                if (!operation.Parameters.Any(p => p.Name == header.Name))
                {
                    operation.Parameters.Add(header);
                }
            }
        }
    }
}
