using AdminBackend.Application.Services.Search;
using AdminBackend.Domain.Constants;

namespace AdminBackend.Infrastructure.Services.Search
{
    public class IndexDefinitionFactory : IIndexDefinitionFactory
    {
        private readonly IEnumerable<IIndexDefinition> indexDefinitions;

        public IndexDefinitionFactory(IEnumerable<IIndexDefinition> indexDefinitions)
        {
            this.indexDefinitions = indexDefinitions;
        }

        public IIndexDefinition? GetIndexDefinition(SearchEngineTypes searchEngineType, string? indexIdentifier)
        {
            //Specific Index
            if (!string.IsNullOrWhiteSpace(indexIdentifier))
            {
                var specificIndex = indexDefinitions.Where(p => p.SearchEngineType == searchEngineType && p.SupportedIndexIdentifiers.Contains(indexIdentifier)).FirstOrDefault();
                if (specificIndex != null)
                {
                    return specificIndex;
                }
            }

            //Generic Index
            return indexDefinitions.Where(p => p.SearchEngineType == searchEngineType && p.SupportedIndexIdentifiers.Count() == 0).FirstOrDefault();
        }
    }
}
