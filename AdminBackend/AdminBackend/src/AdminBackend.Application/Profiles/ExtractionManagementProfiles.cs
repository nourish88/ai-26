using AdminBackend.Application.Dtos;
using AdminBackend.Application.Features.ExtractionManagement.ApplicationExtractorEngineHandlers;
using AdminBackend.Application.Features.ExtractionManagement.ExtractorEngineTypeHandlers;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.Data.Paging;

namespace AdminBackend.Application.Profiles
{
    public class ExtractionManagementProfiles : Profile
    {       
        public ExtractionManagementProfiles()
        {      
            #region [ExtractorEngineType]

            CreateMap<CreateExtractorEngineTypeCommand, ExtractorEngineType>();
            CreateMap<ExtractorEngineType, CreateExtractorEngineTypeCommandResult>();
            CreateMap<ExtractorEngineType, ExtractorEngineTypeDto>().ReverseMap();
            CreateMap<UpdateExtractorEngineTypeCommand, ExtractorEngineType>();
            CreateMap<ExtractorEngineType, UpdateExtractorEngineTypeCommandResult>();
            CreateMap<Paginate<ExtractorEngineType>, PageResponse<ExtractorEngineTypeDto>>().ReverseMap();

            #endregion[END_ExtractorEngineType]

            #region [ApplicationExtractorEngine]

            CreateMap<CreateApplicationExtractorEngineCommand, ApplicationExtractorEngine>();
            CreateMap<ApplicationExtractorEngine, CreateApplicationExtractorEngineCommandResult>();
            CreateMap<ApplicationExtractorEngine, ApplicationExtractorEngineDto>().ReverseMap();
            CreateMap<UpdateApplicationExtractorEngineCommand, ApplicationExtractorEngine>();
            CreateMap<ApplicationExtractorEngine, UpdateApplicationExtractorEngineCommandResult>();
            CreateMap<Paginate<ApplicationExtractorEngine>, PageResponse<ApplicationExtractorEngineDto>>().ReverseMap();

            #endregion[END_ApplicationExtractorEngine]

        }
    }
}
