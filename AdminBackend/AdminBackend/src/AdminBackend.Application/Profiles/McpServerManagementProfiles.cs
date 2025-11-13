using AdminBackend.Application.Dtos;
using AdminBackend.Application.Features.McpServerManagement.McpServerHandlers;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.Data.Paging;

namespace AdminBackend.Application.Profiles
{
    public class McpServerManagementProfiles : Profile
    {
        public McpServerManagementProfiles() 
        {
            #region [McpServer]

            CreateMap<CreateMcpServerCommand, McpServer>();
            CreateMap<McpServer, CreateMcpServerCommandResult>();
            CreateMap<McpServer, McpServerDto>().ReverseMap();
            CreateMap<UpdateMcpServerCommand, McpServer>();
            CreateMap<McpServer, UpdateMcpServerCommandResult>();
            CreateMap<Paginate<McpServer>, PageResponse<McpServerDto>>().ReverseMap();

            #endregion[END_McpServer]        
        }
    }
}
