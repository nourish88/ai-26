namespace ToolGateway.Application.Dtos
{
    public record TodoDto(Guid Id, string Title, string Description, bool IsCompleted);
}
