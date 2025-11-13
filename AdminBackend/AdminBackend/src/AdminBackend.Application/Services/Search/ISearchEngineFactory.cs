using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Services.Search
{
    public interface ISearchEngineFactory
    {
        ISearchEngine? GetEngine(SearchEngineTypes searchEngineType,string? indexIdentifier);
    }
}
