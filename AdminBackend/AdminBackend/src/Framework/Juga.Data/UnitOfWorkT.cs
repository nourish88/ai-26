namespace Juga.Data;

public class UnitOfWork<TContext>(TContext context) : IUnitOfWork<TContext>
    where TContext : IUnitOfWork
{
    private readonly TContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public bool CreateDatabase()
    {
        return _context.CreateDatabase();
    }

    public async Task<int> ExecuteSqlRawAsync(string sql)
    {
        var effectedCounts = await _context.ExecuteSqlRawAsync(sql);
        return effectedCounts;
    }

    public int ExecuteSqlRaw(string sql)
    {
        var effectedCounts = _context.ExecuteSqlRaw(sql);
        return effectedCounts;
    }

    public DatabaseFacade GetDataBase()
    {
        var database = _context.GetDataBase();
        return database;
    }

    public async Task<bool> CreateDatabaseAsync()
    {
        return await _context.CreateDatabaseAsync();
    }

    public bool DeleteDatabase()
    {
        return _context.DeleteDatabase();
    }

    public async Task<bool> DeleteDatabaseAsync()
    {
        return await _context.DeleteDatabaseAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        return _context.GetRepository<TEntity>();
    }

    public void OpenConnection()
    {
        _context.OpenConnection();
    }

    public async Task OpenConnectionAsync()
    {
        await _context.OpenConnectionAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await _context.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.RollbackTransactionAsync();
    }

    public IQueryable<T> SqlQueryRaw<T>(string sql)
    {
        return _context.SqlQueryRaw<T>(sql);
    }

    public int SaveChanges(AuditBehaviour auditBehaviour = AuditBehaviour.Enabled)
    {
        return _context.SaveChanges(auditBehaviour);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default,
        AuditBehaviour auditBehaviour = AuditBehaviour.Enabled)
    {
        return await _context.SaveChangesAsync(cancellationToken, auditBehaviour);
    }

    public string GetName()
    {
        throw new NotImplementedException();
    }
}