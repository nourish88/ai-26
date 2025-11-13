using AdminBackend.Application.Business;
using AdminBackend.Application.Features.SearchEngineManagement.SearchHandlers;
using AdminBackend.Application.Repositories;
using AdminBackend.Application.Services.App;
using AdminBackend.Application.Services.FileStorage;
using AutoMapper;
using Juga.Abstractions.Client;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdminBackend.Application.Features.FileManagement.FileHandlers
{

    public record DeleteUserFileCommand(long Id) : ICommand<DeleteUserFileCommandResult>;
    public record DeleteUserFileCommandResult(bool result, string? errorMessage);

    internal class DeleteUserFileCommandHandler(
        ILogger<DeleteUserFileCommandHandler> logger,
        IRepository<Domain.Entities.File> repository,
        IFileStorage fileStorage,
        IApplicationBusiness applicationBusiness,
        IAppService appService,
        IApplicationRepository applicationRepository,
        IUserContextProvider userContextProvider,
        ISender sender,
        IMapper mapper
        )
        : ICommandHandler<DeleteUserFileCommand, DeleteUserFileCommandResult>
    {
        private readonly IRepository<Domain.Entities.File> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteUserFileCommandResult> Handle(DeleteUserFileCommand request, CancellationToken cancellationToken)
        {

            var appIdentifier = appService.RequesterApplicationIdentifier;
            if (appIdentifier == null)
            {
                var message = "App identifier is missing.";
                logger.LogError(message);
                return new DeleteUserFileCommandResult(false, message);
            }
            var application = await applicationRepository.GetByIdentifierAsync(appIdentifier);
            if (application == null)
            {
                var message = $"Application not found for identifier:{appIdentifier}.";
                logger.LogError(message);
                return new DeleteUserFileCommandResult(false, message);
            }

            if (userContextProvider.ClientId == null)
            {
                var message = "User not found.";
                logger.LogError(message);
                return new DeleteUserFileCommandResult(false, message);
            }

            var isUserFileOwner = await applicationBusiness.IsUserFileOwnerInApplication(userContextProvider.ClientId, request.Id, application.Id, cancellationToken);
            if (!isUserFileOwner)
            {
                var message = "User is not file owner.";
                logger.LogError(message);
                return new DeleteUserFileCommandResult(false, message);
            }
            var deleteFileCommand = new DeleteFileCommand(request.Id);
            var deleteResult = await sender.Send(deleteFileCommand, cancellationToken);
            return new DeleteUserFileCommandResult(deleteResult.result, null);
        }
    }
}
