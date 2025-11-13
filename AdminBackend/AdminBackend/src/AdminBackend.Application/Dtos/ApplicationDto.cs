using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Dtos
{
    public record ApplicationDto
    (
        long Id,
        string Name,
        string Identifier,
        bool HasApplicationFile,
        bool HasUserFile,
        bool EnableGuardRails,
        bool CheckHallucination,
        string Description,
        string SystemPrompt,
        ApplicationTypes ApplicationTypeId,
        MemoryTypes MemoryTypeId,
        OutputTypes OutputTypeId
    );
}
