namespace Shopping.Infrastructure.CrossCutting.FileStorageService.Model;

internal class MinioConfiguration
{
    public int ExpiryFileUrlMinute { get; set; }
    public string? BucketName { get; set; } = null!;
}