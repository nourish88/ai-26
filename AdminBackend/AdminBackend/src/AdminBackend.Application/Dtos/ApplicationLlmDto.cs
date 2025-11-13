namespace AdminBackend.Application.Dtos
{
    public record ApplicationLlmDto(
        long Id,
        float TopP,
        float Temperature,
        bool EnableThinking,
        long ApplicationId,
        long LlmId
    );
}
