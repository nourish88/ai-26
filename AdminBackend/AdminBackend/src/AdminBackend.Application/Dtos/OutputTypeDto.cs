using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Dtos
{
    public record OutputTypeDto
     (
        OutputTypes Id,
        string Identifier
    );
}
