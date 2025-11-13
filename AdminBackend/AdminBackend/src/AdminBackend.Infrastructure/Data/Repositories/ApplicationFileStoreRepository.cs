using AdminBackend.Application.Repositories;
using AdminBackend.Domain.Entities;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AdminBackend.Infrastructure.Data.Repositories;

public class ApplicationFileStoreRepository(IMemoryCache memoryCache, IRepository<ApplicationFileStore> repository): IApplicationFileStoreRepository
{
    public IRepository<ApplicationFileStore> Repository => repository;
    
    public async Task<ApplicationFileStore?> GetCachedApplicationFileStore(long applicationId, CancellationToken cancellationToken = default)
    {
        return await memoryCache.GetOrCreateAsync($"{nameof(ApplicationFileStoreRepository)}-{applicationId}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await Repository
                .Where(x => x.ApplicationId == applicationId)
                .AsNoTracking()
                .Include(x => x.FileStore)
                .FirstOrDefaultAsync(cancellationToken);
        });
    }
}