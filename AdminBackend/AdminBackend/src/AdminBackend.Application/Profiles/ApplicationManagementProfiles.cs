using AdminBackend.Application.Dtos;
using AdminBackend.Application.Features.ApplicationManagement.AgentHandlers;
using AdminBackend.Application.Features.ApplicationManagement.ApplicationEmbeddingHandlers;
using AdminBackend.Application.Features.ApplicationManagement.ApplicationHandlers;
using AdminBackend.Application.Features.ApplicationManagement.ApplicationLlmHandlers;
using AdminBackend.Application.Features.ApplicationManagement.ApplicationMcpServerHandlers;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.Data.Paging;

namespace AdminBackend.Application.Profiles
{
    public class ApplicationManagementProfiles : Profile
    {       
        public ApplicationManagementProfiles()
        {      
            #region [Application]

            CreateMap<CreateApplicationCommand, Domain.Entities.Application>();
            CreateMap<Domain.Entities.Application, CreateApplicationCommandResult>();
            CreateMap<Domain.Entities.Application, ApplicationDto>().ReverseMap();
            CreateMap<UpdateApplicationCommand, Domain.Entities.Application>();
            CreateMap<Domain.Entities.Application, UpdateApplicationCommandResult>();
            CreateMap<Paginate<Domain.Entities.Application>, PageResponse<ApplicationDto>>().ReverseMap();

            #endregion[END_Application]

            #region [ApplicationLlm]

            CreateMap<CreateApplicationLlmCommand, ApplicationLlm>();
            CreateMap<ApplicationLlm, CreateApplicationLlmCommandResult>();
            CreateMap<ApplicationLlm, ApplicationLlmDto>().ReverseMap();
            CreateMap<UpdateApplicationLlmCommand, ApplicationLlm>();
            CreateMap<ApplicationLlm, UpdateApplicationLlmCommandResult>();
            CreateMap<Paginate<ApplicationLlm>, PageResponse<ApplicationLlmDto>>().ReverseMap();

            #endregion[END_ApplicationLlm]

            #region [ApplicationEmbedding]

            CreateMap<CreateApplicationEmbeddingCommand, ApplicationEmbedding>();
            CreateMap<ApplicationEmbedding, CreateApplicationEmbeddingCommandResult>();
            CreateMap<ApplicationEmbedding, ApplicationEmbeddingDto>().ReverseMap();
            CreateMap<UpdateApplicationEmbeddingCommand, ApplicationEmbedding>();
            CreateMap<ApplicationEmbedding, UpdateApplicationEmbeddingCommandResult>();
            CreateMap<Paginate<ApplicationEmbedding>, PageResponse<ApplicationEmbeddingDto>>().ReverseMap();

            #endregion[END_ApplicationEmbedding]

            #region [ApplicationType]

            CreateMap<ApplicationType, ApplicationTypeDto>().ReverseMap();
            CreateMap<Paginate<ApplicationType>, PageResponse<ApplicationTypeDto>>().ReverseMap();

            #endregion[END_ApplicationType]

            #region [MemoryType]

            CreateMap<MemoryType, MemoryTypeDto>().ReverseMap();
            CreateMap<Paginate<MemoryType>, PageResponse<MemoryTypeDto>>().ReverseMap();

            #endregion[END_MemoryType]

            #region [OutputType]

            CreateMap<OutputType, OutputTypeDto>().ReverseMap();
            CreateMap<Paginate<OutputType>, PageResponse<OutputTypeDto>>().ReverseMap();

            #endregion[END_OutputType]

            #region [ApplicationMcpServer]

            CreateMap<CreateApplicationMcpServerCommand, ApplicationMcpServer>();
            CreateMap<ApplicationMcpServer, CreateApplicationMcpServerCommandResult>();
            CreateMap<ApplicationMcpServer, ApplicationMcpServerDto>().ReverseMap();
            CreateMap<UpdateApplicationMcpServerCommand, ApplicationMcpServer>();
            CreateMap<ApplicationMcpServer, UpdateApplicationMcpServerCommandResult>();
            CreateMap<Paginate<ApplicationMcpServer>, PageResponse<ApplicationMcpServerDto>>().ReverseMap();

            #endregion[END_ApplicationMcpServer]

            #region [ChatDetection]

            CreateMap<CreateChatDetectionCommand, ChatDetection>();
            CreateMap<ChatDetection, CreateChatDetectionCommandResult>();
            CreateMap<ChatDetection, ChatDetectionDto>().ReverseMap();
            CreateMap<Paginate<ChatDetection>, PageResponse<ChatDetectionDto>>().ReverseMap();

            #endregion[END_ChatDetection]


        }
    }
}
