namespace AdminBackend.Application.Repositories
{
    public interface IApplicationRepository
    {
        public Task<Domain.Entities.Application?> GetByIdentifierAsync(string identifier);
    }
}
