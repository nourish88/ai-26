using AdminBackend.Domain.Entities;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Repositories;

public interface IApplicationFileStoreRepository
{
    IRepository<ApplicationFileStore> Repository { get; }

    Task<ApplicationFileStore?> GetCachedApplicationFileStore(long applicationId,
        CancellationToken cancellationToken = default);
}