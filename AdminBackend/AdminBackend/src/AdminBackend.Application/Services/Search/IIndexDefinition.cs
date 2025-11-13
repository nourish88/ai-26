using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Services.Search
{
    public interface IIndexDefinition
    {
        public SearchEngineTypes SearchEngineType { get; }
        public string[] SupportedIndexIdentifiers { get; }
        public object Definition(string indexName);
    }
}
