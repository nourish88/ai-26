namespace AdminBackend.Application.Dtos
{
    public record ChunkingStrategyDto
    (
        long Id,
        string Identifier,
        bool IsChunkingSizeRequired,
        bool IsOverlapRequired
    );
    
}
