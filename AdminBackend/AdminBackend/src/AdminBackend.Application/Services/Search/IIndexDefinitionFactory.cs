using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Services.Search
{
    public interface IIndexDefinitionFactory
    {
        IIndexDefinition? GetIndexDefinition(SearchEngineTypes searchEngineType, string? indexIdentifier);
    }
}
