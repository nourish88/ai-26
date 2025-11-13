using AdminBackend.Application.Services.Search;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Models.Services.Search;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;

namespace AdminBackend.Infrastructure.Services.Search.IndexDefinitions
{
    public class ElasticSearchIndexDefinition : IIndexDefinition
    {
        public SearchEngineTypes SearchEngineType => SearchEngineTypes.Elastic;

        public string[] SupportedIndexIdentifiers => [];

        public object Definition(string indexName)
        {
            //CreateIndexRequestDescriptor<ElasticSearchEntity> createIndexRequestDescriptor =
            //    new CreateIndexRequestDescriptor<ElasticSearchEntity>(indexName)
            //    .Mappings(
            //        m=>m.Properties(p=>
            //            p.Keyword(x=>x.id,keyword=>keyword.Index(true))
            //            .Keyword(x=>x.parentid,keyword=>keyword.Index(false))
            //            .Keyword(x=>x.datasourceid,keyword=>keyword.Index(false))
            //            .Keyword(x=>x.userid,keyword=>keyword.Index(false))
            //            .Text(x=>x.title,keyword=>keyword.Index(false))
            //            .Keyword(x => x.sourceurl, keyword => keyword.Index(false))
            //            .Keyword(x => x.storeidentifier, keyword => keyword.Index(false))
            //            .Keyword(x => x.bucket, keyword => keyword.Index(false))
            //            .Keyword(x => x.filepath, keyword => keyword.Index(false))
            //            .Text(x => x.content, keyword => keyword.Index(true))
            //            .Keyword(x => x.keywords, keyword => keyword.Index(false))
            //            .Keyword(x => x.sentiment, keyword => keyword.Index(false))
            //            .IntegerNumber(x=>x.filetype, keyword => keyword.Index(false))
            //            .DenseVector(x=>x.contentvector)
            //        )
            //    );
            var cri = new CreateIndexRequest(indexName);
            cri.Mappings = new TypeMappingDescriptor<ElasticSearchEntity>().Properties(p =>
                        p.Keyword(x => x.id, keyword => keyword.Index(true))
                        .Keyword(x => x.parentid, keyword => keyword.Index(false))
                        .Keyword(x => x.datasourceid, keyword => keyword.Index(false))
                        .Keyword(x => x.userid, keyword => keyword.Index(false))
                        .Text(x => x.title, keyword => keyword.Index(false))
                        .Keyword(x => x.sourceurl, keyword => keyword.Index(false))
                        .Keyword(x => x.storeidentifier, keyword => keyword.Index(false))
                        .Keyword(x => x.bucket, keyword => keyword.Index(false))
                        .Keyword(x => x.filepath, keyword => keyword.Index(false))
                        .Text(x => x.content, keyword => keyword.Index(true))
                        .Keyword(x => x.keywords, keyword => keyword.Index(false))
                        .Keyword(x => x.sentiment, keyword => keyword.Index(false))
                        .IntegerNumber(x => x.filetype, keyword => keyword.Index(false))
                        .DenseVector(x => x.contentvector));

            return cri;
        }
    }
}
