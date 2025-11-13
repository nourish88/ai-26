using AdminBackend.Domain.Constants;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using File = AdminBackend.Domain.Entities.File;

namespace AdminBackend.Application.Features.FileManagement.FileHandlers;

public record FileStatusQuery(long Id):IQuery<FileStatusQueryResult>;

public record FileStatusQueryResult(bool Success, string? Error, long? Id, IngestionStatusTypes? Status);

public class GetFileStatusQueryHandler(IRepository<File> fileRepository): IQueryHandler<FileStatusQuery, FileStatusQueryResult>
{
    public async Task<FileStatusQueryResult> Handle(FileStatusQuery request, CancellationToken cancellationToken)
    {
        var entity = await fileRepository.Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (entity == null)
        {
            return new FileStatusQueryResult(false, "File not found", null, null);
        }
        
        return new FileStatusQueryResult(true, null, entity.Id, entity.IngestionStatusTypeId);
    }
}