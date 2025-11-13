using AdminBackend.Application.Services.Search;
using AdminBackend.Domain.Constants;

namespace AdminBackend.Infrastructure.Services.Search
{
    public class SearchEngineFactory : ISearchEngineFactory
    {
        private readonly IEnumerable<ISearchEngine> searchEngines;

        public SearchEngineFactory(IEnumerable<ISearchEngine> searchEngines)
        {
            this.searchEngines = searchEngines;
        }

        public ISearchEngine? GetEngine(SearchEngineTypes searchEngineType, string? indexIdentifier)
        {
            //Specific Index
            if (!string.IsNullOrWhiteSpace(indexIdentifier))
            {
                var specificIndex = searchEngines.Where(p => p.SearchEngineType == searchEngineType && p.SupportedIndexIdentifiers.Contains(indexIdentifier)).FirstOrDefault();
                if (specificIndex != null)
                {
                    return specificIndex;
                }
            }

            //Generic Index
            return searchEngines.Where(p => p.SearchEngineType == searchEngineType && p.SupportedIndexIdentifiers.Count() == 0).FirstOrDefault();
        }
    }
}
