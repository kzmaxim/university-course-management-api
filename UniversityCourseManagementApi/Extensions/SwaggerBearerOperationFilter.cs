using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UniversityCourseManagementApi.Extensions;

public class SwaggerBearerOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer")] = []
        });
    }
}
