using AdminBackend.Application.Services.FileStorage;
using AdminBackend.Application.Settings;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System;

namespace AdminBackend.Infrastructure.Services.FileStorage;

public class BlobStorage(IMinioClient minioClient, IOptions<FileStorageSettings> fileStorageOptions): IFileStorage
{
    public string GetBucketName(string storageIdentifier)
    {
        if (fileStorageOptions.Value.BucketNameMappings?.TryGetValue(storageIdentifier, out var bucketName) != true)
            throw new ArgumentException($"Storage identifier {storageIdentifier} not found in configuration",
                nameof(storageIdentifier));

        return bucketName!;
    }
    
    private async Task<bool> ExistsAsync(string bucketName, string key,
        CancellationToken cancellationToken = default)
    {
        var arg = new StatObjectArgs()
            .WithObject(key)
            .WithBucket(bucketName);
        
        var stat = await minioClient.StatObjectAsync(arg, cancellationToken);
        return !string.IsNullOrEmpty(stat.ETag);
    }

    public async Task<byte[]> GetObjectAsync(string storageIdentifier, string key, CancellationToken cancellationToken = default)
    {
        var bucketName = GetBucketName(storageIdentifier);

        using var memoryStream = new MemoryStream();
        
        var arg = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(key)
            .WithCallbackStream(stream => { stream.CopyTo(memoryStream); });

        await minioClient.GetObjectAsync(arg, cancellationToken);

        return memoryStream.ToArray();
    }

    public async Task PutObjectAsync(string storageIdentifier, string key, Stream stream, CancellationToken cancellationToken = default)
    {
        var bucketName = GetBucketName(storageIdentifier);

        var arg = new PutObjectArgs()
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithObject(key)
            .WithBucket(bucketName!);

        await minioClient.PutObjectAsync(arg, cancellationToken);
        if (await ExistsAsync(bucketName!, key, cancellationToken) == false)
            throw new Exception($"Failed to upload file {key} to storage {storageIdentifier}");
    }

    public async Task DeleteObjectsAsync(string storageIdentifier, IList<string> keys, CancellationToken cancellationToken = default)
    {
        var bucketName = GetBucketName(storageIdentifier);
        
        var arg = new RemoveObjectsArgs()
            .WithObjects(keys)
            .WithBucket(bucketName);
       await minioClient.RemoveObjectsAsync(arg, cancellationToken);
    }

    public async Task DeleteObjectRecursivelyAsync(string storageIdentifier, string key, CancellationToken cancellationToken = default)
    {
        var objects = await ListObjectsAsync(storageIdentifier, key, cancellationToken);
        if (objects.Any())
        {
            await DeleteObjectsAsync(storageIdentifier, objects.ToList(), cancellationToken);
        }
    }

    public async Task<IReadOnlyList<string>> ListObjectsAsync(string storageIdentifier, string prefix, CancellationToken cancellationToken = default)
    {
        var bucketName = GetBucketName(storageIdentifier);
        var arg = new ListObjectsArgs()
            .WithRecursive(true)
            .WithBucket(bucketName)
            .WithPrefix(prefix);

        var items = minioClient.ListObjectsEnumAsync(arg, cancellationToken);
        var result = new List<string>();
        
        await foreach (var item in items)
        {
            result.Add(item.Key);
        }

        return result;
    }

    
}