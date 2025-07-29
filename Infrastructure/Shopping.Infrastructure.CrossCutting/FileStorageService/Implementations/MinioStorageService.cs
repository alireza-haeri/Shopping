using FileTypeChecker;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Shopping.Application.Contracts.FileService.Interfaces;
using Shopping.Application.Contracts.FileService.Models;
using Shopping.Infrastructure.CrossCutting.FileStorageService.Model;

namespace Shopping.Infrastructure.CrossCutting.FileStorageService.Implementations;

internal class MinioStorageService(IMinioClient minioClient,IOptions<MinioConfiguration> configuration) : IFileService
{
    private readonly MinioConfiguration _minioConfiguration = configuration.Value;
    private const string ShoppingBucketName = "shopping.files";

    private async Task CreateBucketIfMissingAsync(CancellationToken cancellationToken = default)
    {
        var checkBucketExistArg = new BucketExistsArgs().WithBucket(ShoppingBucketName);
        if (await minioClient.BucketExistsAsync(checkBucketExistArg, cancellationToken))
            return;

        var createBucketArg = new MakeBucketArgs().WithBucket(ShoppingBucketName);
        await minioClient.MakeBucketAsync(createBucketArg, cancellationToken);
    }

    public async Task<List<SaveFileResultModel>> SaveFilesAsync(List<SaveFileModel> files,
        CancellationToken cancellationToken)
    {
        await CreateBucketIfMissingAsync(cancellationToken);

        var result = new List<SaveFileResultModel>();

        foreach (var file in files)
        {
            await using var memoryStream = new MemoryStream(Convert.FromBase64String(file.Base64File));
            var fileName = $"{Guid.NewGuid():N}.{FileTypeValidator.GetFileType(memoryStream).Extension}";
            var fileType = !string.IsNullOrEmpty(file.FileContent) ? file.FileContent : "application/octet-stream";
            
            memoryStream.Position = 0;
            
            var createFileArg = new PutObjectArgs()
                .WithBucket(ShoppingBucketName)
                .WithStreamData(memoryStream)
                .WithObjectSize(memoryStream.Length)
                .WithObject(fileName)
                .WithContentType(fileType);

            await minioClient.PutObjectAsync(createFileArg, cancellationToken);

            result.Add(new SaveFileResultModel(fileName, fileType));
        }

        return result;
    }

    public async Task<List<GetFileModel>> GetFilesByNameAsync(List<string> fileNames,
        CancellationToken cancellationToken)
    {
        await CreateBucketIfMissingAsync(cancellationToken);

        var result = new List<GetFileModel>();

        foreach (var fileName in fileNames)
        {
            var objectInfo = new StatObjectArgs()
                .WithBucket(ShoppingBucketName)
                .WithObject(fileName);

            ObjectStat objectInfoResult;
            
            try
            {
                objectInfoResult = await minioClient.StatObjectAsync(objectInfo, cancellationToken);
            }
            catch (ObjectNotFoundException e)
            {
                continue;
            }

            var sasUrlArgs = new PresignedGetObjectArgs()
                .WithBucket(ShoppingBucketName)
                .WithObject(fileName)
                .WithExpiry((int)TimeSpan.FromMinutes(_minioConfiguration.ExpiryFileUrlMinute).TotalSeconds);

            var fileUrl = await minioClient.PresignedGetObjectAsync(sasUrlArgs);
            
            result.Add(new GetFileModel(fileUrl,objectInfoResult.ContentType,objectInfoResult.ObjectName));
        }

        return result;
    }

    public async Task RemoveFileAsync(string[] fileNames, CancellationToken cancellationToken)
    {
        await CreateBucketIfMissingAsync(cancellationToken);

        var fileNamesToRemove = new List<string>();
        
        foreach (var fileName in fileNames)
        {
            var objectInfo = new StatObjectArgs()
                .WithBucket(ShoppingBucketName)
                .WithObject(fileName);

            var objectInfoResult = await minioClient.StatObjectAsync(objectInfo, cancellationToken);
            if (objectInfoResult is null)
                continue;

            fileNamesToRemove.Add(fileName);
        }

        var removeObjectArg = new RemoveObjectsArgs()
            .WithBucket(ShoppingBucketName)
            .WithObjects(fileNames);
        
        await minioClient.RemoveObjectsAsync(removeObjectArg, cancellationToken);
    }
}