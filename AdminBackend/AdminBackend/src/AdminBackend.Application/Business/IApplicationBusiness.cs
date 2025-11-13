using AdminBackend.Application.Services.AI;

namespace AdminBackend.Application.Business
{
    public interface IApplicationBusiness
    {
        public Task<ApplicationSearchEngineMeta?> GetApplicationSearchEngine(long applicationId, CancellationToken cancellationToken = default);
        public Task<IEmbeddingService?> GetApplicationEmbeddingService(long applicationId, CancellationToken cancellationToken = default);
        public Task<bool> IsUserFileOwner(string userId, long fileId, CancellationToken cancellationToken = default);
        public Task<bool> IsUserFileOwnerInApplication(string userId, long fileId, long applicationId, CancellationToken cancellationToken = default);
    }
}
