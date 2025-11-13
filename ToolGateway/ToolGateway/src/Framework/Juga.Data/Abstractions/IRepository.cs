namespace Juga.Data.Abstractions;

/// <summary>
/// Belirli bir tipteki entitynin veri tabanı işlemlerini yönetilmesine olanak sağlar.
/// </summary>
/// <typeparam name="TEntity">Entity</typeparam>
public interface IRepository<TEntity> : IQueryable<TEntity> where TEntity : class
{
    #region[CRUD_OPERATIONS]
    /// <summary>
    /// Veri tabanına kayıt işlemi.
    /// </summary>
    /// <param name="entity">Kayıt edilmek istenilen entity.</param>
    /// <param name="insertStrategy">Kayıt işlemi sırasında izlenecek strateji.</param>
    /// <returns>Kayıt işleminden sonra kaydedilen entitynin son hali.</returns>
    TEntity Insert(TEntity entity, InsertStrategy insertStrategy = InsertStrategy.InsertAll);

    /// <summary>
    /// Veri tabanı kayıt işlemi.
    /// </summary>
    /// <param name="entities">Kayıt edilmek istenen entity arrayi.</param>
    void Insert(params TEntity[] entities);

    /// <summary>
    /// Veri tabanı kayıt işlemi.
    /// </summary>
    /// <param name="entities">Kayıt edilmek istenen entity listesi.</param>
    void Insert(IEnumerable<TEntity> entities);

    /// <summary>
    /// Veri tabanına kayıt işlemi.
    /// </summary>
    /// <param name="entity">Kayıt edilmek istenilen entity.</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Kayıt işleminden sonra kaydedilen entitynin son hali.</returns>
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken), bool detach = false);

    /// <summary>
    /// Veri tabanı kayıt işlemi.
    /// </summary>
    /// <param name="entities">Kayıt edilmek istenen entity arrayi.</param>
    /// <returns>Async operasyon</returns>
    Task InsertAsync(params TEntity[] entities);

    Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

    void Update(TEntity entity, UpdateStrategy updateStrategy = UpdateStrategy.UpdateAll);

    void Update(params TEntity[] entities);

    void Update(IEnumerable<TEntity> entities);

    void Delete(object id);

    void Delete(TEntity entity, DeleteStrategy deleteStrategy = DeleteStrategy.MainIfRequiredAddChilds);
    // Task<TEntity> DeleteAsync(TEntity entity, DeleteStrategy deleteStrategy = DeleteStrategy.MainIfRequiredAddChilds );
    void DeleteRange(params TEntity[] entities);

    void DeleteRange(IEnumerable<TEntity> entities);

    #endregion[END_CRUD_OPERATIONS]

    #region[QUERY_OPERATIONS]

    IQueryable<TEntity> GetAll();

    int BulkUpdate(Expression<Func<TEntity, bool>> predicate,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> updateExpression);

    Task<int> BulkUpdateAsync(
       Expression<Func<TEntity, bool>> predicate,
       Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> updateExpression);
    Task<int> BulkDeleteAsync(
        Expression<Func<TEntity, bool>> predicate);

    int BulkDelete(Expression<Func<TEntity, bool>> predicate);
    IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault);

    Task<IList<TEntity>> GetAllAsync(bool? asNoTracking = false);

    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false,
        bool enableTracking = true, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetListByDynamicAsync(DynamicQuery dynamic, Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,

        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);
    Task<Paginate<TEntity>> GetPaginatedListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );

    Task<Paginate<TEntity>> GetPaginatedListByDynamicAsync(DynamicQuery dynamic, Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int index = 0, int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default);

    Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault);

    TEntity Find(params object[] keyValues);

    ValueTask<TEntity> FindAsync(params object[] keyValues);

    ValueTask<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken);

    TEntity GetFirstOrDefault(Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault);

    TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault);

    Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,

        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault, CancellationToken cancellationToken = default);

    Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        TrackingBehaviour tracking = TrackingBehaviour.ContextDefault, CancellationToken cancellationToken = default);

    IQueryable<TEntity> FromSql(string sql, params object[] parameters);
    bool Exists(Expression<Func<TEntity, bool>> selector = null);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> selector = null);

    #endregion[END_QUERY_OPERATIONS]

    #region[AGGRIGATIONS]

    int Count(Expression<Func<TEntity, bool>> predicate = null);

    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);

    long LongCount(Expression<Func<TEntity, bool>> predicate = null);

    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate = null);

    T Max<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

    Task<T> MaxAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

    T Min<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

    Task<T> MinAsync<T>(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, T>> selector = null);

    decimal Average(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

    Task<decimal> AverageAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

    decimal Sum(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

    Task<decimal> SumAsync(Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, decimal>> selector = null);

    #endregion[END_AGGRIGATIONS]           

    #region[CONTEXT_OPERATIONS]

    void Detach(TEntity entity);

    #endregion[END_CONTEXT_OPERATIONS]     

    #region[SQL_COMMAND_OPERATIONS]

    /// <summary>
    /// Raw Sql Calıştırmak için kullanılır.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns>Etkilenen Kayıt Sayısı</returns>
    int ExecuteSqlRaw(string sql, params object[] parameters);
    /// <summary>
    /// Raw Sql Calıştırmak için kullanılır.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns>Etkilenen Kayıt Sayısı</returns>
    int ExecuteSqlRaw(string sql, IEnumerable<object> parameters);
    /// <summary>
    /// Raw Sql Calıştırmak için kullanılır.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns>Etkilenen Kayıt Sayısı</returns>
    Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
    /// <summary>
    /// Raw Sql Calıştırmak için kullanılır.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns>Etkilenen Kayıt Sayısı</returns>
    Task<int> ExecuteSqlRawAsync(string sql, IEnumerable<object> parameters);

    #endregion[SQL_COMMAND_OPERATIONS]

    #region[UNITOFWORK_OPERATIONS]
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken? cancellationToken=null);
    #endregion[UNITOFWORK_OPERATIONS]


    void BeginTransaction();


    void Commit();


    void Rollback();

}