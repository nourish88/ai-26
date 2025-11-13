using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Dtos
{
    public record FileTypeDto
    (
        FileTypes Id,
        string Identifier
    );
}
