using AdminBackend.Application.Business;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Microsoft.Extensions.Logging;

namespace AdminBackend.Application.Features.SearchEngineManagement.IndexManagementHandlers;

public record DeleteIndexCommand(long ApplicationId) : ICommand<DeleteIndexCommandResult>;

public record DeleteIndexCommandResult(bool Success);

public class DeleteIndexCommandValidator : AbstractValidator<DeleteIndexCommand>
{
    public DeleteIndexCommandValidator()
    {
    }
}

public class DeleteIndexCommandHandler(
    ILogger<DeleteIndexCommandHandler> logger,
    IApplicationBusiness applicationBusiness
) : ICommandHandler<DeleteIndexCommand, DeleteIndexCommandResult>
{
    public async Task<DeleteIndexCommandResult> Handle(DeleteIndexCommand request, CancellationToken cancellationToken)
    {

        var searchEngineMeta = await applicationBusiness.GetApplicationSearchEngine(request.ApplicationId, cancellationToken);
        if (searchEngineMeta == null)
        {
            return new DeleteIndexCommandResult(false);
        }

        var searchEngine = searchEngineMeta.SearchEngine;
        var indexName = searchEngineMeta.IndexName;
        return new DeleteIndexCommandResult(await searchEngine.DeleteIndexAsync(indexName, cancellationToken));
    }
}