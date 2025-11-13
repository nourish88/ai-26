namespace Juga.Data;

public abstract class UnitOfWork : DbContext, IUnitOfWork
{
    private readonly UnitOfWorkOptions _unitOfWorkOptions;
    private readonly IConfiguration configuration;
    private readonly IServiceProvider serviceProvider;
    private Dictionary<Type, object> repositories;
    private readonly IAuditPropertyInterceptorManager _auditPropertyInterceptorManager;
    private readonly IAuditBehaviourService auditBehaviourService;
    private static readonly ILoggerFactory DebuggerLoggerFactory
        = LoggerFactory.Create(builder =>
        {
            builder.AddFilter((catagory, level) =>
                    catagory == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information)
                .AddDebug();
        });
     

    /// <summary>
    /// Migration ve EF Power Tools tarafından kullanılmak üzere oluşturulmuştur.
    /// </summary>
    /// 

    protected UnitOfWork(IServiceProvider service)
    {
        serviceProvider = service;
       
    }
    public UnitOfWork(DatabaseType databaseType = DatabaseType.SqlServer) : base()
    {
        _unitOfWorkOptions = new InternalUnitOfWorkOptions(databaseType);
        _auditPropertyInterceptorManager = new InternalAuditPropertyInterceptorManager(_unitOfWorkOptions);
    }

    /// <summary>
    /// Migration ve EF Power Tools tarafından kullanılmak üzere oluşturulmuştur.
    /// </summary>
    public UnitOfWork(DbContextOptions dbContextOptions, DatabaseType databaseType = DatabaseType.SqlServer) : base(dbContextOptions)
    {
        _unitOfWorkOptions = new InternalUnitOfWorkOptions(databaseType);
        _auditPropertyInterceptorManager = new InternalAuditPropertyInterceptorManager(_unitOfWorkOptions);
    }

    public UnitOfWork(
        IOptions<UnitOfWorkOptions> options,
        IAuditPropertyInterceptorManager auditPropertyInterceptorManager, IConfiguration configuration)
        : base()
    {
        _unitOfWorkOptions = options.Value;
        _auditPropertyInterceptorManager = auditPropertyInterceptorManager;
        this.configuration = configuration;


        ConfigureContext();
    }

    public UnitOfWork(
        IOptions<UnitOfWorkOptions> options,
        [NotNull] DbContextOptions dbContextOptions,
        IAuditPropertyInterceptorManager auditPropertyInterceptorManager,
        IAuditBehaviourService auditBehaviourService, IConfiguration configuration)
        : base(dbContextOptions)
    {
        _unitOfWorkOptions = options.Value;
        _auditPropertyInterceptorManager = auditPropertyInterceptorManager;
        this.auditBehaviourService = auditBehaviourService;
        this.configuration = configuration;
        ConfigureContext();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_unitOfWorkOptions is { EnableDebuggerLogger: true })
        {
            optionsBuilder.UseLoggerFactory(DebuggerLoggerFactory).EnableSensitiveDataLogging();
        }


        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            SetPropertyInterceptors(modelBuilder, entityType);
            SetDefaultDeleteBehavior(entityType);
        }
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContext).Assembly);

        if (configuration != null)
        {
            var queueConfigSection = configuration.GetSection("Juga:Queue");

            if (queueConfigSection != null)
            {
                var outboxType = queueConfigSection.GetValue<OutboxType>("OutboxType");
                if (outboxType != OutboxType.None)
                {
                    SetOutboxMigration(modelBuilder);
                }
            }


        }

        base.OnModelCreating(modelBuilder);
    }
    public enum QueueType
    {
        None = 0,
        RabbitMQ = 1
    }
    public enum OutboxType
    {
        None = 0,
        SqlServer = 1,
        Postgres = 2,

    }
    public void SetOutboxMigration(ModelBuilder modelBuilder)
    {
        if (configuration != null)
        {
            var queueConfigSection = configuration.GetSection("Juga:Queue");
            var isQueueEnabled = queueConfigSection.GetValue<bool>("IsEnabled");
            var queueType = queueConfigSection.GetValue<QueueType>("QueueType");

            var isQueueSetEnabledAndSet = queueConfigSection != null && isQueueEnabled && queueType != QueueType.None;
            if (isQueueSetEnabledAndSet)
            {
                modelBuilder.AddInboxStateEntity();
                modelBuilder.AddOutboxMessageEntity();
                modelBuilder.AddOutboxStateEntity();
            }
        }

    }

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        repositories ??= new Dictionary<Type, object>();

        var type = typeof(TEntity);
        if (!repositories.ContainsKey(type))
        {
            if (_unitOfWorkOptions == null || _unitOfWorkOptions.RunAsDisconnected)
            {
                repositories[type] = new DisconnectedRepository<TEntity>(this, serviceProvider);
            }
            else
            {
                repositories[type] = new ConnectedRepository<TEntity>(this, serviceProvider);
            }
        }

        return (IRepository<TEntity>)repositories[type];
    }

    public virtual int SaveChanges(AuditBehaviour auditBehaviour = AuditBehaviour.Enabled)
    {
        auditBehaviourService.AuditBehaviour = auditBehaviour;
        if (_unitOfWorkOptions.RunSaveChangesAsTransactional)
        {
            using (var transactionScope = new TransactionScope())
            {
                var result = base.SaveChanges();
                transactionScope.Complete();
                return result;
            }
        }
        else
        {
            return base.SaveChanges();
        }

    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, AuditBehaviour auditBehaviour = AuditBehaviour.Enabled)
    {
        auditBehaviourService.AuditBehaviour = auditBehaviour;
        if (_unitOfWorkOptions.RunSaveChangesAsTransactional)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await base.SaveChangesAsync(cancellationToken);
                transactionScope.Complete();
                return result;
            }
        }
        else
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }

    private void ConfigureContext()
    {
       
        if (_unitOfWorkOptions == null || _unitOfWorkOptions.RunAsDisconnected)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
    }

    #region[ENTITY_MODEL_CREATION]

    private void SetPropertyInterceptors(ModelBuilder modelBuilder, IMutableEntityType entityType)
    {
        _auditPropertyInterceptorManager?.OnModelCreating(modelBuilder.Entity(entityType.Name));
    }

    private void SetDefaultDeleteBehavior(IMutableEntityType entityType)
    {
        if (_unitOfWorkOptions != null)
        {
            var foreignKeys = entityType.GetForeignKeys();
            if (foreignKeys.Any())
            {
                foreach (var fk in foreignKeys)
                {
                    fk.DeleteBehavior = _unitOfWorkOptions.DefaultDeleteBehavior;
                }
            }
        }
    }

    #endregion[END_ENTITY_MODEL_CREATION]

    #region[DATABASE_OPERATIONS]

    public bool CreateDatabase()
    {
        if (_unitOfWorkOptions is { EnableDatabaseCreation: true })
        {
            return base.Database.EnsureCreated();
        }
        return false;
    }

    public bool DeleteDatabase()
    {
        if (_unitOfWorkOptions is { EnableDatabaseDeletion: true })
        {
            return base.Database.EnsureDeleted();
        }
        return false;
    }

    public void OpenConnection()
    {
        base.Database.OpenConnection();
    }

    public async Task<bool> CreateDatabaseAsync()
    {
        if (_unitOfWorkOptions is { EnableDatabaseCreation: true })
        {
            return await base.Database.EnsureCreatedAsync();
        }
        return false;
    }

    public async Task<bool> DeleteDatabaseAsync()
    {
        if (_unitOfWorkOptions is { EnableDatabaseDeletion: true })
        {
            return await base.Database.EnsureDeletedAsync();
        }
        return false;
    }

    public IQueryable<T> SqlQueryRaw<T>(string sql)
    {
        return base.Database.SqlQueryRaw<T>($"{sql}");
    }
    public async Task<int> ExecuteSqlRawAsync(string sql)
    {
        var effectedCounts = await base.Database.ExecuteSqlRawAsync(sql);
        return effectedCounts;
    }
    public int ExecuteSqlRaw(string sql)
    {
        var effectedCounts = base.Database.ExecuteSqlRaw(sql);
        return effectedCounts;
    }
    public DatabaseFacade GetDataBase()
    {
        var database = base.Database;
        return database;
    }

    public async Task OpenConnectionAsync()
    {
        await base.Database.OpenConnectionAsync();
    }

    public async Task BeginTransactionAsync()
    {

        await base.Database.BeginTransactionAsync();
    }
    public DbContext GetContext()
    {

        return this;
    }
    public async Task RollbackTransactionAsync()
    {

        await base.Database.RollbackTransactionAsync();
    }
    public async Task CommitTransactionAsync()
    {

        await base.Database.CommitTransactionAsync();
    }

    public string GetName()
    {
       return this.GetContext().GetType().Name;
    }
    #endregion[END_DATABASE_OPERATIONS]
}