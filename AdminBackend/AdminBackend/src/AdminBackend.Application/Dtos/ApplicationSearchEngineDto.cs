namespace AdminBackend.Application.Dtos
{
    public record ApplicationSearchEngineDto
    (
        long Id,
        long ApplicationId,
        long SearchEngineId,
        string IndexName,
        long EmbeddingId,
        string Identifier
    );
}
