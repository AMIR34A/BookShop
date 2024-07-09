using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookShop.Areas.API.Swagger;

public class RemoveVersionParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var versionParameter = operation.Parameters.FirstOrDefault(parameter => parameter.Name.Equals("version"));
        if (versionParameter is not null)
            operation.Parameters.Remove(versionParameter);
    }
}
