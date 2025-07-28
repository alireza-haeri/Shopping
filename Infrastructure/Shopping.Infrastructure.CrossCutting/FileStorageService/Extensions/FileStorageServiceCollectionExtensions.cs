using Microsoft.Extensions.DependencyInjection;
using Minio;
using Shopping.Application.Contracts.FileService.Interfaces;
using Shopping.Infrastructure.CrossCutting.FileStorageService.Implementations;
using Shopping.Infrastructure.CrossCutting.FileStorageService.Model;

namespace Shopping.Infrastructure.CrossCutting.FileStorageService.Extensions;

public static class FileStorageServiceCollectionExtensions
{
    public static IServiceCollection AddFileStorageServices(this IServiceCollection services,
        AddFileStorageServicesModel model)
    {
        services.AddMinio(options =>
        {
            options
                .WithCredentials(model.AccessKey, model.SecretKey)
                .WithEndpoint(model.Endpoint)
                .WithSSL(model.UseSsl);
        });

        services.AddScoped<IFileService, MinioStorageService>();
        services.Configure<MinioConfiguration>(configuration =>
        {
            configuration.ExpiryFileUrlMinute = model.ExpiryFileUrlMinute;
        });

        return services;
    }
}

public record AddFileStorageServicesModel(
    string AccessKey,
    string SecretKey,
    string Endpoint,
    bool UseSsl,
    int ExpiryFileUrlMinute);