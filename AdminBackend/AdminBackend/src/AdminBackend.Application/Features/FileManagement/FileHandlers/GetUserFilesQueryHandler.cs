using AdminBackend.Application.Dtos;
using AdminBackend.Application.Repositories;
using AutoMapper;
using Juga.Abstractions.Client;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.Extensions.Logging;

namespace AdminBackend.Application.Features.FileManagement.FileHandlers
{
    public record UserFilesQuery(string applicationIdentifier) : IQuery<UserFilesQueryResult>;
    public record UserFilesQueryResult(List<FileDto>? result);
    internal class GetUserFilesQueryHandler(
        ILogger<GetUserFilesQueryHandler> logger,
        IRepository<Domain.Entities.File> repository,
        IMapper mapper,
        IUserContextProvider userContextProvider,
        IApplicationRepository applicationRepository
        )
        : IQueryHandler<UserFilesQuery, UserFilesQueryResult>
    {
        private readonly IRepository<Domain.Entities.File> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UserFilesQueryResult> Handle(UserFilesQuery request, CancellationToken cancellationToken)
        {
            var applicationEntity = await applicationRepository.GetByIdentifierAsync(request.applicationIdentifier);
            if (applicationEntity == null)
            {
                logger.LogError($"Application({request.applicationIdentifier}) not found.");
                return new UserFilesQueryResult(null);
            }
            var userId = userContextProvider.ClientId;
            if (userId == null)
            {
                logger.LogError("User id not found");
                return new UserFilesQueryResult(null);
            }
            var entitiy = await repository.GetAllAsync(predicate: p => p.CreatedBy == userId && p.UploadApplicationId == applicationEntity.Id);
            var dto = mapper.Map<List<FileDto>>(entitiy);
            return new UserFilesQueryResult(dto);
        }
    }
}
