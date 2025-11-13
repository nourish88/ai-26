using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Dtos
{
    public record IngestionStatusTypeDto
    (
        IngestionStatusTypes Id,
        string Identifier
    );
}
