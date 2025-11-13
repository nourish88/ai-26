using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Dtos
{
    public record ApplicationTypeDto
    (
        ApplicationTypes Id,
        string Identifier
    );
}
