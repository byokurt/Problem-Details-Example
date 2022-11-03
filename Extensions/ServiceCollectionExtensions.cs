using Microsoft.OpenApi.Models;

namespace ProblemDetailsExample.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "Demo API", Version = "v1"});
            c.SwaggerDoc("v2", new OpenApiInfo {Title = "Demo API", Version = "v2"});
        });
    }
}