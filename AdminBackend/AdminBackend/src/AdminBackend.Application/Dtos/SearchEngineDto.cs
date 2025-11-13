namespace AdminBackend.Application.Dtos
{
    public record SearchEngineDto
    (
        long Id,
        long SearchEngineTypeId,
        string Identifier,
        string Url
    );
    
}
