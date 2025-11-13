using AdminBackend.Application.Dtos;
using AdminBackend.Application.Features.ChunkingManagement.ApplicationChunkingStrategyHandlers;
using AdminBackend.Application.Features.ChunkingManagement.ChunkingStrategyHandlers;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.Data.Paging;

namespace AdminBackend.Application.Profiles
{
    public class ChunkingStrategyManagementProfiles : Profile
    {       
        public ChunkingStrategyManagementProfiles()
        {      
            #region [ChunkingStrategy]

            CreateMap<CreateChunkingStrategyCommand, ChunkingStrategy>();
            CreateMap<ChunkingStrategy, CreateChunkingStrategyCommandResult>();
            CreateMap<ChunkingStrategy, ChunkingStrategyDto>().ReverseMap();
            CreateMap<UpdateChunkingStrategyCommand, ChunkingStrategy>();
            CreateMap<ChunkingStrategy, UpdateChunkingStrategyCommandResult>();
            CreateMap<Paginate<ChunkingStrategy>, PageResponse<ChunkingStrategyDto>>().ReverseMap();

            #endregion[END_ChunkingStrategy]

            #region [ApplicationChunkingStrategy]

            CreateMap<CreateApplicationChunkingStrategyCommand, ApplicationChunkingStrategy>();
            CreateMap<ApplicationChunkingStrategy, CreateApplicationChunkingStrategyCommandResult>();
            CreateMap<ApplicationChunkingStrategy, ApplicationChunkingStrategyDto>().ReverseMap();
            CreateMap<UpdateApplicationChunkingStrategyCommand, ApplicationChunkingStrategy>();
            CreateMap<ApplicationChunkingStrategy, UpdateApplicationChunkingStrategyCommandResult>();
            CreateMap<Paginate<ApplicationChunkingStrategy>, PageResponse<ApplicationChunkingStrategyDto>>().ReverseMap();

            #endregion[END_ApplicationChunkingStrategy]
        }
    }
}
