using AdminBackend.Domain.Constants;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using File = AdminBackend.Domain.Entities.File;

namespace AdminBackend.Application.Features.FileManagement.FileHandlers;

public record UpdateFileErrorStatusCommand(long Id, string ErrorDetail) : ICommand<UpdateFileErrorStatusCommandResult>;

public record UpdateFileErrorStatusCommandResult(bool Success, string? Error);

public class UpdateFileErrorStatusCommandValidator : AbstractValidator<UpdateFileErrorStatusCommand>
{
    public UpdateFileErrorStatusCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.ErrorDetail).NotNull().NotEmpty();
    }
}

public class UpdateFileErrorStatusCommandHandler(IRepository<File> fileRepository)
    : ICommandHandler<UpdateFileErrorStatusCommand, UpdateFileErrorStatusCommandResult>
{
    public async Task<UpdateFileErrorStatusCommandResult> Handle(UpdateFileErrorStatusCommand request,
        CancellationToken cancellationToken)
    {
        var file = await fileRepository
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id,
            cancellationToken: cancellationToken);
        
        if (file == null)
        {
            return new UpdateFileErrorStatusCommandResult(false, "File not found");
        }

        file.IngestionStatusTypeId = IngestionStatusTypes.ProcessingFailed;
        file.ErrorDetail = request.ErrorDetail.Substring(0, 
            request.ErrorDetail.Length > 2000 ? 2000 : request.ErrorDetail.Length );
        fileRepository.Update(file);
        await fileRepository.SaveChangesAsync(cancellationToken);

        return new UpdateFileErrorStatusCommandResult(true, null);
    }
}