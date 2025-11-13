namespace AdminBackend.Application.Services.FileStorage;

public interface IFileStorage
{
    string GetBucketName(string storageIdentifier);
    Task<byte[]> GetObjectAsync(string storageIdentifier, string key, CancellationToken cancellationToken = default);
    Task PutObjectAsync(string storageIdentifier, string key, Stream stream, CancellationToken cancellationToken = default);
    Task DeleteObjectsAsync(string storageIdentifier, IList<string> keys, CancellationToken cancellationToken = default);
    Task DeleteObjectRecursivelyAsync(string storageIdentifier, string key, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> ListObjectsAsync(string storageIdentifier, string prefix,
        CancellationToken cancellationToken = default);
}