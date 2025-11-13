using System.Text.Json.Serialization;

namespace AdminBackend.Application.Services.DataManager;

public record JobRequest
{
    [JsonPropertyName("application_id")]
    public required long ApplicationId { get; init; }
    [JsonPropertyName("file_id")]
    public required long FileId { get; init; }
    [JsonPropertyName("file_store_id")]
    public required long FileStoreId { get; init; }
    [JsonPropertyName("document_id")]
    public required string DocumentId { get; init; }
    [JsonPropertyName("file_extension")]
    public required string FileExtension { get; init; }
    [JsonPropertyName("bucket_name")]
    public string? BucketName { get; init; }
    [JsonPropertyName("storage_type")]
    public required string StorageType { get; init; }  // s3
    [JsonPropertyName("extractor_type")]
    public required string ExtractorType { get; init; } // markitdown
    [JsonPropertyName("chunk_type")]
    public required string ChunkType { get; init; } // markdown, fixed-size, html
    [JsonPropertyName("chunk_size")]
    public required int ChunkSize { get; init; }
    [JsonPropertyName("chunk_overlap")]
    public required int ChunkOverlap { get; init; }
}