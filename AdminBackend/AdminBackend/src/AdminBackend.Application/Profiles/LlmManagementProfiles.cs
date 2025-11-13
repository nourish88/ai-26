using AdminBackend.Application.Dtos;
using AdminBackend.Application.Features.LlmManagement.EmbeddingHandlers;
using AdminBackend.Application.Features.LlmManagement.LlmHandlers;
using AdminBackend.Application.Features.LlmManagement.LlmProviderHandlers;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.Data.Paging;

namespace AdminBackend.Application.Profiles
{
    public class LlmManagementProfiles : Profile
    {       
        public LlmManagementProfiles()
        {
            

            #region [LlmProvider]

            CreateMap<CreateLlmProviderCommand, LlmProvider>();
            CreateMap<LlmProvider, CreateLlmProviderCommandResult>();
            CreateMap<LlmProvider, LlmProviderDto>().ReverseMap();
            CreateMap<UpdateLlmProviderCommand, LlmProvider>();
            CreateMap<LlmProvider, UpdateLlmProviderCommandResult>();
            CreateMap<Paginate<LlmProvider>, PageResponse<LlmProviderDto>>().ReverseMap();

            #endregion[END_LlmProvider]

            #region [Llm]

            CreateMap<CreateLlmCommand, Llm>();
            CreateMap<Llm, CreateLlmCommandResult>();
            CreateMap<Llm, LlmDto>().ReverseMap();
            CreateMap<UpdateLlmCommand, Llm>();
            CreateMap<Llm, UpdateLlmCommandResult>();
            CreateMap<Paginate<Llm>, PageResponse<LlmDto>>().ReverseMap();

            #endregion[END_Llm]

            #region [Embedding]

            CreateMap<CreateEmbeddingCommand, Embedding>();
            CreateMap<Embedding, CreateEmbeddingCommandResult>();
            CreateMap<Embedding, EmbeddingDto>().ReverseMap();
            CreateMap<UpdateEmbeddingCommand, Embedding>();
            CreateMap<Embedding, UpdateEmbeddingCommandResult>();
            CreateMap<Paginate<Embedding>, PageResponse<EmbeddingDto>>().ReverseMap();

            #endregion[END_Embedding]
        }
    }
}
