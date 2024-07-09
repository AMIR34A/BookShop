using Microsoft.OpenApi.Models;
using NuGet.Packaging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BookShop.Areas.API.Swagger;

public class SetVersionInPath : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        Dictionary<string, OpenApiPathItem> updatedPath = new();

        foreach (var path in swaggerDoc.Paths)
            updatedPath.Add(path.Key.Replace("v{version}", swaggerDoc.Info.Version), path.Value);

        swaggerDoc.Paths.Clear();
       swaggerDoc.Paths.AddRange(updatedPath);
    }
}
