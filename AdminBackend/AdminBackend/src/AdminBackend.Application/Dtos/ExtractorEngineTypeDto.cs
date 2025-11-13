namespace AdminBackend.Application.Dtos
{
    public record ExtractorEngineTypeDto
    (
        long Id,
        string Identifier,
        bool Word,
        bool Txt,
        bool Pdf
    );
}
