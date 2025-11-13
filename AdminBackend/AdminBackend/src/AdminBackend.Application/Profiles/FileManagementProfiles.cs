using AdminBackend.Application.Dtos;
using AdminBackend.Application.Features.FileManagement.ApplicationFileStoreHandlers;
using AdminBackend.Application.Features.FileManagement.FileHandlers;
using AdminBackend.Application.Features.FileManagement.FileStoreHandlers;
using AdminBackend.Application.Features.FileManagement.FileTypeHandlers;
using AdminBackend.Application.Features.FileManagement.IngestionStatusTypeHandlers;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.Data.Paging;

namespace AdminBackend.Application.Profiles
{
    public class FileManagementProfiles : Profile
    {       
        public FileManagementProfiles()
        {      
            #region [IngestionStatusType]

            CreateMap<CreateIngestionStatusTypeCommand, IngestionStatusType>();
            CreateMap<IngestionStatusType, CreateIngestionStatusTypeCommandResult>();
            CreateMap<IngestionStatusType, IngestionStatusTypeDto>().ReverseMap();
            CreateMap<UpdateIngestionStatusTypeCommand, IngestionStatusType>();
            CreateMap<IngestionStatusType, UpdateIngestionStatusTypeCommandResult>();
            CreateMap<Paginate<IngestionStatusType>, PageResponse<IngestionStatusTypeDto>>().ReverseMap();

            #endregion[END_IngestionStatusType]

            #region [FileType]

            CreateMap<CreateFileTypeCommand, FileType>();
            CreateMap<FileType, CreateFileTypeCommandResult>();
            CreateMap<FileType, FileTypeDto>().ReverseMap();
            CreateMap<UpdateFileTypeCommand, FileType>();
            CreateMap<FileType, UpdateFileTypeCommandResult>();
            CreateMap<Paginate<FileType>, PageResponse<FileTypeDto>>().ReverseMap();

            #endregion[END_FileType]

            #region [FileStore]

            CreateMap<CreateFileStoreCommand, FileStore>();
            CreateMap<FileStore, CreateFileStoreCommandResult>();
            CreateMap<FileStore, FileStoreDto>().ReverseMap();
            CreateMap<UpdateFileStoreCommand, FileStore>();
            CreateMap<FileStore, UpdateFileStoreCommandResult>();
            CreateMap<Paginate<FileStore>, PageResponse<FileStoreDto>>().ReverseMap();

            #endregion[END_FileStore]

            #region [ApplicationFileStore]

            CreateMap<CreateApplicationFileStoreCommand, ApplicationFileStore>();
            CreateMap<ApplicationFileStore, CreateApplicationFileStoreCommandResult>();
            CreateMap<ApplicationFileStore, ApplicationFileStoreDto>().ReverseMap();
            CreateMap<UpdateApplicationFileStoreCommand, ApplicationFileStore>();
            CreateMap<ApplicationFileStore, UpdateApplicationFileStoreCommandResult>();
            CreateMap<Paginate<ApplicationFileStore>, PageResponse<ApplicationFileStoreDto>>().ReverseMap();

            #endregion[END_ApplicationFileStore]

            #region [File]

            CreateMap<CreateFileCommand, Domain.Entities.File>();
            CreateMap<Domain.Entities.File, CreateFileCommandResult>();
            CreateMap<Domain.Entities.File, FileDto>().ReverseMap();
            CreateMap<UpdateFileCommand, Domain.Entities.File>();
            CreateMap<Domain.Entities.File, UpdateFileCommandResult>();
            CreateMap<Paginate<Domain.Entities.File>, PageResponse<FileDto>>().ReverseMap();

            #endregion[END_File]
        }
    }
}
