using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Dtos
{
    public record MemoryTypeDto
    (
        MemoryTypes Id,
        string Identifier
    );
}
