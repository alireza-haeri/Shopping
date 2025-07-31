using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace Shopping.WebFramework.Swagger;

public static class SwaggerConfigurationExtensions
{
    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        var versions = builder.Configuration
            .GetSection("Swagger")
            .GetSection("Versions")
            .Get<string[]>();

        if (versions is null || versions.Length == 0)
            throw new ArgumentNullException($"SwaggerVersions");

        foreach (var version in versions)
        {
            builder.Services.AddOpenApiDocument(options =>
            {
                options.Title = "Shopping API";
                options.Version = version;
                options.DocumentName = version;

                options.AddSecurity("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "Enter JWT Token ONLY",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Type = OpenApiSecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                options.OperationProcessors
                    .Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));

                options.DocumentProcessors.Add(new ApiVersionDocumentProcessor());
            });
        }

        return builder;
    }

    public static WebApplication UseSwagger(this WebApplication app)
    {
        if (app.Environment.IsProduction())
            return app;

        app.UseOpenApi();

        app.UseSwaggerUi(options =>
        {
            options.PersistAuthorization = true;
            options.EnableTryItOut = true;

            options.Path = "/Swagger";
        });

        app.UseReDoc(settings =>
        {
            settings.Path = "/api-docs/{documentName}";
            settings.DocumentTitle = "Shopping API Documentation";
        });

        return app;
    }
}