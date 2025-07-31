using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Shopping.Application.Contracts.FileService.Interfaces;
using Shopping.Infrastructure.CrossCutting.FileStorageService.Extensions;
using Shopping.Infrastructure.CrossCutting.FileStorageService.Model;
using Testcontainers.Minio;
using Xunit;

namespace Shopping.Infrastructure.FileStorage.Minio.Test
{
    public class MinioFileStorageTestSetup : IAsyncLifetime
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;
        public IFileService FileService { get; private set; } = null!;
        // BucketName را در دسترس کلاس‌های تست قرار می‌دهیم
        public string BucketName { get; }

        private readonly MinioContainer _minioContainer = new MinioBuilder()
            .WithImage("minio/minio:latest")
            .WithUsername("minioadmin")
            .WithPassword("minioadmin")
            .Build();

        public MinioFileStorageTestSetup()
        {
            // یک نام تصادفی و منحصر به فرد برای Bucket در هر بار اجرای مجموعه تست‌ها
            BucketName = $"test-bucket-{Guid.NewGuid():N}";
        }

        public async Task InitializeAsync()
        {
            await _minioContainer.StartAsync();

            var hostname = _minioContainer.Hostname;
            var port = _minioContainer.GetMappedPublicPort(9000);
            var endpoint = $"{hostname}:{port}";

            var serviceCollection = new ServiceCollection();

            // *** تغییر کلیدی: تزریق BucketName تصادفی به سرویس ***
            serviceCollection.AddFileStorageServices(new AddFileStorageServicesModel(
                "minioadmin",
                "minioadmin",
                endpoint,
                false,
                2,
                BucketName)); // <-- BucketName اینجا تزریق می‌شود

            // IMinioClient را هم برای استفاده در Base Class ثبت می‌کنیم
            serviceCollection.AddSingleton(sp => 
                sp.GetRequiredService<IMinioClientFactory>().CreateClient());

            ServiceProvider = serviceCollection.BuildServiceProvider(false);
            FileService = ServiceProvider.GetRequiredService<IFileService>();
        }

        public async Task DisposeAsync()
        {
            await _minioContainer.StopAsync();
        }
    }
}