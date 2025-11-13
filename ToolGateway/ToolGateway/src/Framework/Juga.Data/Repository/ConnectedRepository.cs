namespace Juga.Data.Repository;

public class ConnectedRepository<TEntity> : BaseRepository<TEntity>, IRepository<TEntity> where TEntity : class
{
    public ConnectedRepository(IUnitOfWork dbContext,IServiceProvider service) : base(dbContext, service)
    {
    }

    #region[CRUD_OPERATIONS]

    public override void Delete(object id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            Delete(entity);
        }
    }

    #endregion[END_CRUD_OPERATIONS]

    #region[QUERY_OPERATIONS]        

    #endregion[END_QUERY_OPERATIONS]

    protected override IQueryable<TEntity> SetTracking(IQueryable<TEntity> query, TrackingBehaviour tracking)
    {
        if (tracking == TrackingBehaviour.AsNoTracking)
        {
            query = query.AsNoTracking();
        }
        return query;
    }
}