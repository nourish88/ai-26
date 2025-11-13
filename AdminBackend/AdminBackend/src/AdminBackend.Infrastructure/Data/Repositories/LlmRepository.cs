using AdminBackend.Application.Repositories;
using AdminBackend.Domain.Entities;
using Juga.Data.Abstractions;

namespace AdminBackend.Infrastructure.Data.Repositories
{
    public class LlmRepository(IRepository<Llm> repository) : ILlmRepository
    {
    }
}
