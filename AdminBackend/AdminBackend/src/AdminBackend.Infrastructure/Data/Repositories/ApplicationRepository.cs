using AdminBackend.Application.Repositories;
using Juga.Data.Abstractions;
using System.Data.Entity;

namespace AdminBackend.Infrastructure.Data.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly IRepository<Domain.Entities.Application> repository;

        public ApplicationRepository(IRepository<Domain.Entities.Application> repository)
        {
            this.repository = repository;
        }

        public async Task<Domain.Entities.Application?> GetByIdentifierAsync(string identifier)
        {
            var entities = await repository.GetAllAsync(predicate: p => p.Identifier == identifier);
            if (entities.Any())
            {
                var entity = entities.SingleOrDefault();//x.Identifier must be unique
                return entity;
            }
            else
            {
                return null;
            }
        }
    }
}
