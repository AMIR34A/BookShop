using Asp.Versioning;
using BookShop.Areas.API.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace BookShop.Areas.API.Swagger;

public static class SwaggerConfigurationExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(setup =>
        {
            setup.SwaggerDoc("v1", new OpenApiInfo()
            {
                Version = "v1",
                Title = "BookShop Api",
                Description = "An api for CRUD operations",
            });

            setup.DescribeAllParametersInCamelCase();
            setup.OperationFilter<RemoveVersionParameter>();
            setup.DocumentFilter<SetVersionInPath>();

            setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = " Authorization",
                In = ParameterLocation.Header
            });

            setup.DocInclusionPredicate((apiName, apiDescription) =>
            {
                if (!apiDescription.TryGetMethodInfo(out MethodInfo methodInfo))
                    return false;
                var versions = methodInfo.DeclaringType.
                                  GetCustomAttribute<ApiVersionAttribute>(true).Versions;

                return versions.Any(apiVersion => apiName.Equals($"v{apiVersion.MajorVersion}"));
            });

            //var path = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //var xmlPath = Path.Combine(AppContext.BaseDirectory, path);
            //setup.IncludeXmlComments(() => new System.Xml.XPath.XPathDocument(xmlPath));
        });
    }

    public static void AddSwaggerAndSwaggerUI(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(setup =>
        {
            setup.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 API");
        });
    }
}
