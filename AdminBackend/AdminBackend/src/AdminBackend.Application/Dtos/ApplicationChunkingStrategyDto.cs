namespace AdminBackend.Application.Dtos
{
    public record ApplicationChunkingStrategyDto
    (
        long Id,
        long ApplicationId,
        long ChunkingStrategyId,
        int? ChunkSize,
        int? Overlap,
        string? Seperator
    );
}
