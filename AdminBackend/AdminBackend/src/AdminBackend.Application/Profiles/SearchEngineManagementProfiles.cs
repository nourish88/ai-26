using AdminBackend.Application.Dtos;
using AdminBackend.Application.Features.SearchEngineManagement.ApplicationSearchEngineHandlers;
using AdminBackend.Application.Features.SearchEngineManagement.SearchEngineHandlers;
using AdminBackend.Application.Features.SearchEngineManagement.SearchEngineTypeHandlers;
using AdminBackend.Application.Features.SearchEngineManagement.SearchHandlers;
using AdminBackend.Domain.Entities;
using AdminBackend.Domain.Models.Services.Search;
using AutoMapper;
using Juga.Data.Paging;

namespace AdminBackend.Application.Profiles
{
    public class SearchEngineManagementProfiles : Profile
    {       
        public SearchEngineManagementProfiles()
        {         
            #region [SearchEngineType]

            CreateMap<CreateSearchEngineTypeCommand, SearchEngineType>();
            CreateMap<SearchEngineType, CreateSearchEngineTypeCommandResult>();
            CreateMap<SearchEngineType, SearchEngineTypeDto>().ReverseMap();
            CreateMap<UpdateSearchEngineTypeCommand, SearchEngineType>();
            CreateMap<SearchEngineType, UpdateSearchEngineTypeCommandResult>();
            CreateMap<Paginate<SearchEngineType>, PageResponse<SearchEngineTypeDto>>().ReverseMap();

            #endregion[END_SearchEngineType]

            #region [SearchEngine]

            CreateMap<CreateSearchEngineCommand, SearchEngine>();
            CreateMap<SearchEngine, CreateSearchEngineCommandResult>();
            CreateMap<SearchEngine, SearchEngineDto>().ReverseMap();
            CreateMap<UpdateSearchEngineCommand, SearchEngine>();
            CreateMap<SearchEngine, UpdateSearchEngineCommandResult>();
            CreateMap<Paginate<SearchEngine>, PageResponse<SearchEngineDto>>().ReverseMap();

            #endregion[END_SearchEngine]

            #region [ApplicationSearchEngine]

            CreateMap<CreateApplicationSearchEngineCommand, ApplicationSearchEngine>();
            CreateMap<ApplicationSearchEngine, CreateApplicationSearchEngineCommandResult>();
            CreateMap<ApplicationSearchEngine, ApplicationSearchEngineDto>().ReverseMap();
            CreateMap<UpdateApplicationSearchEngineCommand, ApplicationSearchEngine>();
            CreateMap<ApplicationSearchEngine, UpdateApplicationSearchEngineCommandResult>();
            CreateMap<Paginate<ApplicationSearchEngine>, PageResponse<ApplicationSearchEngineDto>>().ReverseMap();

            #endregion[END_ApplicationSearchEngine]

            #region [SemanticSearch]

            CreateMap<SemanticSearchOutput, SemanticSearchQueryResult>();
            CreateMap<SemanticSearchOutput, SemanticSearchQueryResponse>()
                .ConstructUsing(x=> new SemanticSearchQueryResponse(
                    x.id,
                    x.parentid,
                    x.datasourceid,
                    x.title,
                    x.sourceurl,
                    x.content,
                    x.sentiment
                    ));

            #endregion[END_SemanticSearch]
        }
    }
}
