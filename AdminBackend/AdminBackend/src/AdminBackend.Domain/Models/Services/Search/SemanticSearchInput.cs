namespace AdminBackend.Domain.Models.Services.Search
{
    public record SemanticSearchInput
    (
        string query,
        int? top,
        int? skip,
        string applicationId,
        string userId,
        string[]? fileIdentifiers
    );
}
