using FluentAssertions;
using Minio;
using Minio.DataModel.Args;
using Shopping.Application.Contracts.FileService.Interfaces;
using Shopping.Application.Contracts.FileService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Minio.ApiEndpoints;
using Xunit;

namespace Shopping.Infrastructure.FileStorage.Minio.Test
{
    /// <summary>
    /// Base class for Minio tests, handling cleanup of the isolated test bucket.
    /// </summary>
    public abstract class MinioFileStorageTestBase : IClassFixture<MinioFileStorageTestSetup>, IAsyncLifetime
    {
        protected readonly IFileService FileService;
        private readonly IMinioClient _minioClient;
        protected readonly string BucketName;

        protected MinioFileStorageTestBase(MinioFileStorageTestSetup setup)
        {
            FileService = setup.FileService;
            BucketName = setup.BucketName;
            _minioClient = setup.ServiceProvider.GetRequiredService<IMinioClient>();
        }

        protected string GetFakeBase64Image() => "R0lGODlhAQABAIABAP8AAP///yH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==";

        protected List<SaveFileModel> CreateSaveFileModels(int count)
        {
            return Enumerable.Range(0, count)
                .Select(_ => new SaveFileModel(GetFakeBase64Image(), "image/gif"))
                .ToList();
        }

        // InitializeAsync دیگر نیازی به کد ندارد چون Bucket توسط سرویس ساخته می‌شود
        public Task InitializeAsync() => Task.CompletedTask;

        [Obsolete("Obsolete")]
        public async Task DisposeAsync()
        {
            // Clean up by removing all objects and the bucket itself after the test class has run.
            try
            {
                var bucketExistsArgs = new BucketExistsArgs().WithBucket(BucketName);
                if (!await _minioClient.BucketExistsAsync(bucketExistsArgs)) return;

                var objectsArgs = new ListObjectsArgs().WithBucket(BucketName);
                var objectsObservable = _minioClient.ListObjectsAsync(objectsArgs);
                var objects = await objectsObservable.ToList();

                if (objects.Any())
                {
                    var removeObjectsArgs = new RemoveObjectsArgs()
                        .WithBucket(BucketName)
                        .WithObjects(objects.Select(o => o.Key).ToList());
                    await _minioClient.RemoveObjectsAsync(removeObjectsArgs);
                }

                var removeBucketArgs = new RemoveBucketArgs().WithBucket(BucketName);
                await _minioClient.RemoveBucketAsync(removeBucketArgs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Minio cleanup: {ex.Message}");
            }
        }
    }

    public class MinioFileStorageIntegrationTests
    {
        public class SaveAndGetFilesTests : MinioFileStorageTestBase
        {
            public SaveAndGetFilesTests(MinioFileStorageTestSetup setup) : base(setup) { }

            [Fact]
            public async Task SaveFilesAsync_WithValidModels_ShouldReturnSuccessResults()
            {
                // Arrange
                var filesToSave = CreateSaveFileModels(1);

                // Act
                var result = await FileService.SaveFilesAsync(filesToSave, CancellationToken.None);

                // Assert
                result.Should().HaveCount(1);
                result[0].FileName.Should().NotBeNullOrEmpty().And.EndWith(".gif");
            }

            [Fact]
            public async Task GetFilesByNameAsync_WhenFilesExist_ShouldReturnFileModelsWithUrls()
            {
                // Arrange
                var savedFiles = await FileService.SaveFilesAsync(CreateSaveFileModels(2), CancellationToken.None);
                var fileNamesToGet = savedFiles.Select(f => f.FileName).ToList();

                // Act
                var result = await FileService.GetFilesByNameAsync(fileNamesToGet, CancellationToken.None);

                // Assert
                result.Should().HaveCount(2);
                result.Should().OnlyContain(f => f.FileUrl.Contains(BucketName) && f.FileUrl.Contains(f.FileName));
            }
        }

        public class RemoveFilesTests : MinioFileStorageTestBase
        {
            public RemoveFilesTests(MinioFileStorageTestSetup setup) : base(setup) { }

            [Fact]
            public async Task RemoveFileAsync_WhenFilesExist_ShouldDeleteThemFromStorage()
            {
                // Arrange
                var savedFiles = await FileService.SaveFilesAsync(CreateSaveFileModels(2), CancellationToken.None);
                var fileNamesToRemove = savedFiles.Select(f => f.FileName).ToArray();

                // Act
                await FileService.RemoveFileAsync(fileNamesToRemove, CancellationToken.None);

                // Assert
                var result = await FileService.GetFilesByNameAsync(fileNamesToRemove.ToList(), CancellationToken.None);
                result.Should().BeEmpty();
            }
        }
    }
}