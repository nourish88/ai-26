namespace AdminBackend.Application.Dtos
{
    public record EmbeddingDto
    (
        long Id,
        long LlmProviderId,
        string Url,
        string ModelName,
        int VectorSize,
        int MaxInputTokenSize
    );
}
