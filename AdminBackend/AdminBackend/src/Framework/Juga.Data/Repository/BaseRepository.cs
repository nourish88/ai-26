


using System.Collections;

namespace Juga.Data.Repository;

public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly UnitOfWork _dbContext;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(IUnitOfWork dbContext,IServiceProvider serviceProvider)
    {
        if (serviceProvider !=null)
        {
            var type = typeof(TEntity);
            var services = serviceProvider.GetServices<IUnitOfWork>();
            var context = services.First(
                o=>((DbContext)o).Model.GetEntityTypes()
                .Any(a=>a.ClrType.FullName == type.FullName));
            var contextService = serviceProvider.GetKeyedService<IUnitOfWork>(context.GetName());
            _dbContext = contextService == null ? throw new ArgumentNullException(nameof(contextService)) : contextService as UnitOfWork;
        }
        else
        {

            _dbContext = dbContext == null ? throw new ArgumentNullException(nameof(dbContext)) : dbContext as UnitOfWork;
        }

        if (_dbContext != null) _dbSet = _dbContext.Set<TEntity>();
    }

    #region[CRUD_OPERATIONS]

    public virtual TEntity Insert(TEntity entity, InsertStrategy insertStrategy = InsertStrategy.InsertAll)
    {
        return _dbSet.Add(entity).Entity;
    }

    public void Insert(params TEntity[] entities)
    {
        _dbSet.AddRange(entities);
    }

    public void Insert(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
    }

    public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken), bool detached = false)
    {
        var res = await _dbSet.AddAsync(entity, cancellationToken);

        if (detached)
        {
            res.State = EntityState.Detached;
        }

        return res.Entity;
    }

    public async Task InsertAsync(params TEntity[] entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public async Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(TEntity entity, UpdateStrategy updateStrategy = UpdateStrategy.UpdateAll)
    {
        _dbSet.Update(entity);
    }

    public void Update(params TEntity[] entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Update(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public abstract void Delete(object id);

    public virtual void Delete(TEntity entity, DeleteStrategy deleteStrategy = DeleteStrategy.MainIfRequiredAddChilds)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteRange(params TEntity[] entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public void DeleteRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    protected void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> action)
    {
        _dbContext.ChangeTracker.TrackGraph(rootEntity, action);
    }

    #endregion[END_CRUD_OPERATIONS]

    #region[QUERY_OPERATIONS]

    public IQueryable<TEntity> GetAll()
    {
        return _dbSet;
    }

    public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault)
    {
        IQueryable<TEntity> query = _dbSet;

        query = SetTracking(query, tracking);

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return orderBy != null ? orderBy(query) : query;
    }
    public async Task<int> BulkUpdateAsync(Expression<Func<TEntity, bool>> predicate,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> updateExpression)
    {
        return await _dbSet.Where(predicate).ExecuteUpdateAsync(updateExpression);
    }
    public int BulkUpdate(Expression<Func<TEntity, bool>> predicate,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> updateExpression)
    {
        return _dbSet.Where(predicate).ExecuteUpdate(updateExpression);
    }
    public int BulkDelete(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet.Where(predicate).ExecuteDelete();
    }
    public async Task<int> BulkDeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ExecuteDeleteAsync();
    }
    public async Task<IList<TEntity>> GetAllAsync(bool? asNoTracking = false)
    {

        return await _dbSet.ToListAsync();
    }
    public async Task<Paginate<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = _dbSet;
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        if (orderBy != null)
            return await orderBy(queryable).ToPaginateAsync(index, size, cancellationToken);
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }




    public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = _dbSet;
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        if (orderBy != null)
            return await orderBy(queryable).ToListAsync(cancellationToken);
        return await queryable.ToListAsync(cancellationToken);
    }


    public async Task<Paginate<TEntity>> GetPaginatedListByDynamicAsync(DynamicQuery dynamic, Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = _dbSet;
        queryable = queryable.ToDynamic(dynamic);
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        if (orderBy != null)
            return await orderBy(queryable).ToPaginateAsync(index, size, cancellationToken);
        return await queryable.ToPaginateAsync(index, size, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetListByDynamicAsync(DynamicQuery dynamic, Expression<Func<TEntity, bool>>? predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool withDeleted = false, bool enableTracking = true,
        CancellationToken cancellationToken = default)
    {

        IQueryable<TEntity> queryable = _dbSet;
        queryable = queryable.ToDynamic(dynamic);
        if (!enableTracking)
            queryable = queryable.AsNoTracking();
        if (include != null)
            queryable = include(queryable);
        if (withDeleted)
            queryable = queryable.IgnoreQueryFilters();
        if (predicate != null)
            queryable = queryable.Where(predicate);
        if (orderBy != null)
            return await orderBy(queryable).ToListAsync(cancellationToken);
        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault)
    {
        IQueryable<TEntity> query = _dbSet;

        query = SetTracking(query, tracking);

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return orderBy != null ? await orderBy(query).ToListAsync() : await query.ToListAsync();
    }


    public TEntity Find(params object[] keyValues)
    {
        return _dbSet.Find(keyValues);
    }

    public ValueTask<TEntity> FindAsync(params object[] keyValues)
    {
        return _dbSet.FindAsync(keyValues);
    }

    public ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken)
    {
        return _dbSet.FindAsync(keyValues, cancellationToken);
    }

    public TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault)
    {
        IQueryable<TEntity> query = _dbSet;

        query = SetTracking(query, tracking);

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return orderBy != null ? orderBy(query).FirstOrDefault() : query.FirstOrDefault();
    }

    public TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault)
    {
        IQueryable<TEntity> query = _dbSet;

        query = SetTracking(query, tracking);

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return orderBy != null ? orderBy(query).Select(selector).FirstOrDefault() : query.Select(selector).FirstOrDefault();
    }

    public async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        query = SetTracking(query, tracking);

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return orderBy != null ? await orderBy(query).Select(selector).FirstOrDefaultAsync(cancellationToken) : await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        query = SetTracking(query, tracking);

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return orderBy != null ? await orderBy(query).FirstOrDefaultAsync(cancellationToken) : await query.FirstOrDefaultAsync(cancellationToken);
    }

    public IQueryable<TEntity> FromSql(string sql, params object[] parameters)
    {
        return _dbSet.FromSqlRaw(sql, parameters);
    }

    public bool Exists(Expression<Func<TEntity, bool>> selector = null)
    {
        return selector == null ? _dbSet.Any() : _dbSet.Any(selector);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null)
    {
        return selector == null ? await _dbSet.AnyAsync() : await _dbSet.AnyAsync(selector);
    }


    #endregion[END_QUERY_OPERATIONS]

    #region[AGGRIGATIONS]

    public int Count(Expression<Func<TEntity, bool>> predicate = null)
    {
        return predicate == null ? _dbSet.Count() : _dbSet.Count(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
    {
        return predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);
    }

    public long LongCount(Expression<Func<TEntity, bool>> predicate = null)
    {
        return predicate == null ? _dbSet.LongCount() : _dbSet.LongCount(predicate);
    }

    public async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null)
    {
        return predicate == null ? await _dbSet.LongCountAsync() : await _dbSet.LongCountAsync(predicate);
    }

    public T Max<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
    {
        return predicate == null ? _dbSet.Max(selector) : _dbSet.Where(predicate).Max(selector);
    }

    public async Task<T> MaxAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
    {
        return predicate == null
            ? await _dbSet.MaxAsync(selector)
            : await _dbSet.Where(predicate).MaxAsync(selector);
    }

    public T Min<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
    {
        return predicate == null ? _dbSet.Min(selector) : _dbSet.Where(predicate).Min(selector);
    }

    public async Task<T> MinAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null)
    {
        return predicate == null
            ? await _dbSet.MinAsync(selector)
            : await _dbSet.Where(predicate).MinAsync(selector);
    }

    public decimal Average(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
    {
        return predicate == null ? _dbSet.Average(selector) : _dbSet.Where(predicate).Average(selector);
    }

    public async Task<decimal> AverageAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
    {
        if (predicate == null)
        {
            return await _dbSet.AverageAsync(selector);
        }

        return await _dbSet.Where(predicate).AverageAsync(selector);
    }

    public decimal Sum(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
    {
        return predicate == null ? _dbSet.Sum(selector) : _dbSet.Where(predicate).Sum(selector);
    }

    public async Task<decimal> SumAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null)
    {
        if (predicate == null)
        {
            return await _dbSet.SumAsync(selector);
        }

        return await _dbSet.Where(predicate).SumAsync(selector);
    }

    #endregion[END_AGGRIGATIONS]           

    #region[CONTEXT_OPERATIONS]

    public void Detach(TEntity entity)
    {
        var entry = GetEntityEntry(entity);
        if (entry != null)
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
        }
    }

    #endregion[END_CONTEXT_OPERATIONS]


    #region[SQL_COMMAND_OPERATIONS]

    /// <summary>
    /// Raw Sql Calıştırmak için kullanılır.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns>Etkilenen Kayıt Sayısı</returns>
    public int ExecuteSqlRaw(string sql, params object[] parameters)
    {
        return _dbContext.Database.ExecuteSqlRaw(sql, parameters);
    }

    /// <summary>
    /// Raw Sql Calıştırmak için kullanılır.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns>Etkilenen Kayıt Sayısı</returns>
    public int ExecuteSqlRaw(string sql, IEnumerable<object> parameters)
    {
        return _dbContext.Database.ExecuteSqlRaw(sql, parameters);
    }
    public void BeginTransaction()
    {
        _dbContext.Database.BeginTransaction();
    }

    public void Commit()
    {
        _dbContext.Database.CommitTransaction();
    }

    public void Rollback()
    {
        _dbContext.Database.RollbackTransaction();
    }

    /// <summary>
    /// Raw Sql Calıştırmak için kullanılır.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns>Etkilenen Kayıt Sayısı</returns>
    public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        return await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    /// <summary>
    /// Raw Sql Calıştırmak için kullanılır.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns>Etkilenen Kayıt Sayısı</returns>
    public async Task<int> ExecuteSqlRawAsync(string sql, IEnumerable<object> parameters)
    {
        return await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
    }

    #endregion[SQL_COMMAND_OPERATIONS]

    #region [UNITOFWORK_OPERATIONS]
    public int SaveChanges()
    {
        return _dbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync(CancellationToken? cancellationToken=default)
    {
        return cancellationToken.HasValue
            ? await _dbContext.SaveChangesAsync(cancellationToken.Value)
            : await _dbContext.SaveChangesAsync();
    }
    #endregion [UNITOFWORK_OPERATIONS]


    protected abstract IQueryable<TEntity> SetTracking(IQueryable<TEntity> query, TrackingBehaviour tracking);

    protected EntityEntry<TEntity> GetEntityEntry(TEntity entity)
    {
        return _dbContext.Entry(entity);
    }

    #region[IQUERYABLE_IMPLEMENTATION]
    public Type ElementType => SetTracking(_dbSet, TrackingBehaviour.ContextDefault).ElementType;

    public Expression Expression => SetTracking(_dbSet, TrackingBehaviour.ContextDefault).Expression;

    public IQueryProvider Provider => SetTracking(_dbSet, TrackingBehaviour.ContextDefault).Provider;

    public IEnumerator<TEntity> GetEnumerator()
    {
        return SetTracking(_dbSet, TrackingBehaviour.ContextDefault).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return SetTracking(_dbSet, TrackingBehaviour.ContextDefault).GetEnumerator();
    }
    #endregion[END_IQUERYABLE_IMPLEMENTATION]
}