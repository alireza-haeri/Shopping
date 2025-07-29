using Microsoft.Extensions.DependencyInjection;
using Minio;
using Shopping.Application.Contracts.FileService.Interfaces;
using Shopping.Infrastructure.CrossCutting.FileStorageService.Extensions;
using Testcontainers.Minio;

namespace Shopping.Infrastructure.FileStorage.Minio.Test;

public class MinioFileStorageTestSetup : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; set; } = null!;
    public IFileService FileService { get; set; } = null!;
    
    private readonly MinioContainer _minioContainer = new MinioBuilder()
        .WithImage("minio/minio:latest")
        .WithName("minio-test")
        .WithUsername("minioadmin")
        .WithPassword("minioadmin")
        .WithPortBinding(9000)
        .Build();
    
    public async Task InitializeAsync()
    {
        try
        {
            await _minioContainer.StartAsync();
        }
        catch (Exception e)
        {
            var logs = await _minioContainer.GetLogsAsync();
            Console.WriteLine($"Minio Logs: {logs}");
            throw;
        }

        var endpoint = $"localhost:9000";

        var serviceCollection = new ServiceCollection()
            .AddFileStorageServices(new AddFileStorageServicesModel(
                "minioadmin",
                "minioadmin",
                endpoint,
                false,
                2));
        
        ServiceProvider = serviceCollection.BuildServiceProvider(false);
        
        FileService = ServiceProvider.GetRequiredService<IFileService>();
    }

    public async Task DisposeAsync()
    {
        await _minioContainer.StopAsync();
        await _minioContainer.DisposeAsync();
    }
}