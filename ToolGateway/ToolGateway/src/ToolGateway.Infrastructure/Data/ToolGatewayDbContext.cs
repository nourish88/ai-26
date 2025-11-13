using Juga.Abstractions.Data.AuditLog;
using Juga.Data;
using Juga.Data.AuditProperties;
using Juga.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ToolGateway.Domain.Entities;

namespace ToolGateway.Infrastructure.Data
{
    public class ToolGatewayDbContext : UnitOfWork
    {
        public ToolGatewayDbContext(
        IOptions<UnitOfWorkOptions> options,
        IAuditPropertyInterceptorManager auditPropertyInterceptorManager,
        DbContextOptions<ToolGatewayDbContext> dbContextOptions,
        IAuditBehaviourService auditBehaviourService,
        IConfiguration configuration)
        : base(options, dbContextOptions, auditPropertyInterceptorManager, auditBehaviourService, configuration)
        {



        }
        public ToolGatewayDbContext(DbContextOptions<ToolGatewayDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Todo> Todos => Set<Todo>();
    }
}
