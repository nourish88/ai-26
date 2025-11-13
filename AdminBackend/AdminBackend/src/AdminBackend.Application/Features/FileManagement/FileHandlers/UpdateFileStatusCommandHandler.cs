using System.Threading.Channels;
using AdminBackend.Application.Features.Ingestion;
using AdminBackend.Domain.Constants;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using File = AdminBackend.Domain.Entities.File;

namespace AdminBackend.Application.Features.FileManagement.FileHandlers;

public record UpdateFileStatusCommand(long Id, IngestionStatusTypes Status) : ICommand<UpdateFileStatusCommandResult>;

public record UpdateFileStatusCommandResult(bool Success, string? Error, long? Id, IngestionStatusTypes? CurrentStatus);

public class UpdateFileStatusCommandValidator : AbstractValidator<UpdateFileStatusCommand>
{
    public UpdateFileStatusCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Status).IsInEnum();
    }
}

public class UpdateFileStatusCommandHandler(
    Channel<IndexFileCommand> channel,
    IRepository<File> fileRepository,
    ISender sender)
    : ICommandHandler<UpdateFileStatusCommand, UpdateFileStatusCommandResult>
{
    public async Task<UpdateFileStatusCommandResult> Handle(UpdateFileStatusCommand request,
        CancellationToken cancellationToken)
    {
        var file = await fileRepository.GetFirstOrDefaultAsync(
            predicate: p => p.Id == request.Id,
            include: f => f.Include(i => i.FileType)
            , cancellationToken: cancellationToken);
        
        if (file == null)
        {
            return new UpdateFileStatusCommandResult(false, "File not found", null, null);
        }

        if (file.IngestionStatusTypeId != request.Status)
        {
            // TODO: Status transitions can be checked 
            file.IngestionStatusTypeId = request.Status;
            fileRepository.Update(file);
            await fileRepository.SaveChangesAsync(cancellationToken);
            
            if (request.Status == IngestionStatusTypes.Indexing)
            {
                // Send the command to IndexingJobWorker for async execution
                // This only blocks when the channel's capacity is full.
                await channel.Writer.WriteAsync(new IndexFileCommand(file), cancellationToken);
            }
        }

        return new UpdateFileStatusCommandResult(true, null, file.Id, file.IngestionStatusTypeId);
    }
}