using HackZone.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace HackZone.Infrastructure.Services;

public class MinioFileStorageService(IMinioClient minio, IConfiguration config, ILogger<MinioFileStorageService> logger) : IFileStorageService
{
    private readonly string _bucket = config["Minio:BucketName"] ?? "hackzone";
    private readonly string _endpoint = config["Minio:Endpoint"] ?? "minio:9000";

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        await EnsureBucketAsync(ct);
        var objectName = $"{Guid.NewGuid()}_{fileName}";
        await minio.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType), ct);
        return objectName;
    }

    public async Task DeleteAsync(string objectName, CancellationToken ct = default)
    {
        await minio.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucket).WithObject(objectName), ct);
    }

    public string GetUrl(string objectName) => $"http://{_endpoint}/{_bucket}/{objectName}";

    private async Task EnsureBucketAsync(CancellationToken ct)
    {
        var exists = await minio.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_bucket), ct);
        if (!exists)
            await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket), ct);
    }
}
