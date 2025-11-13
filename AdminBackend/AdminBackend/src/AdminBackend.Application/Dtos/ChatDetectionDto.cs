using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Dtos
{
    public record ChatDetectionDto
    (
        long Id,
        string ApplicationIdentifier,
        ChatDetectionTypes ChatDetectionTypeId,
        string ThreadId,
        string MessageId,
        string UserMessage,
        string? Sources,
        string? Reason
    );
}
