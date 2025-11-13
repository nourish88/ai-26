using System.Reflection;

namespace Juga.Data.Repository;

public class DisconnectedRepository<TEntity>(IUnitOfWork dbContext,IServiceProvider service) : BaseRepository<TEntity>(dbContext, service),
    IRepository<TEntity>
    where TEntity : class
{
    #region[CRUD_OPERATIONS]

    public override TEntity Insert(TEntity entity, InsertStrategy insertStrategy = InsertStrategy.InsertAll)
    {
        switch (insertStrategy)
        {
            case InsertStrategy.InsertAll:
                {
                    return _dbSet.Add(entity).Entity;
                }
            case InsertStrategy.MainIfRequiredChilds:
                {
                    var entityEntry = base._dbContext.Attach(entity);
                    entityEntry.State = EntityState.Added;
                    return entityEntry.Entity;
                }
            case InsertStrategy.OnlytMain:
                {
                    var entityEntry = GetEntityEntry(entity);
                    entityEntry.State = EntityState.Added;
                    return entityEntry.Entity;
                }
            default:
                {
                    throw new NotSupportedException($"Not supported insertStrategy: {insertStrategy}");
                }
        }
    }

    public override void Update(TEntity entity, UpdateStrategy updateStrategy = UpdateStrategy.UpdateAll)
    {
        switch (updateStrategy)
        {
            case UpdateStrategy.UpdateAll:
                {
                    _dbSet.Update(entity);
                    break;
                }
            case UpdateStrategy.MainIfRequiredAddChilds:
                {
                    base._dbContext.Attach(entity).State = EntityState.Modified;
                    break;
                }
            case UpdateStrategy.OnlyMain:
                {
                    GetEntityEntry(entity).State = EntityState.Modified;
                    break;
                }
            default:
                {
                    throw new NotSupportedException($"Not supported updateStrategy: {updateStrategy}");
                }
        }
    }

    public override void Delete(TEntity entity, DeleteStrategy deleteStrategy = DeleteStrategy.MainIfRequiredAddChilds)
    {
        switch (deleteStrategy)
        {
            case DeleteStrategy.DeleteAll:
                {
                    TrackGraph(entity, act =>
                    {
                        if (act.Entry.IsKeySet)
                        {
                            act.Entry.State = EntityState.Deleted;
                        }
                    });
                    break;
                }
            case DeleteStrategy.MainIfRequiredAddChilds:
                {
                    _dbSet.Remove(entity);
                    break;
                }
            case DeleteStrategy.OnlyMain:
                {
                    GetEntityEntry(entity).State = EntityState.Deleted;
                    break;
                }
            default:
                {
                    throw new NotSupportedException($"Not supported updateStrategy: {deleteStrategy}");
                }
        }
    }

    public override void Delete(object id)
    {
        var typeInfo = typeof(TEntity).GetTypeInfo();
        var key = _dbContext.Model.FindEntityType(typeInfo).FindPrimaryKey().Properties.FirstOrDefault();
        var property = typeInfo.GetProperty(key?.Name);
        if (property != null)
        {
            var entity = Activator.CreateInstance<TEntity>();
            property.SetValue(entity, id);
            GetEntityEntry(entity).State = EntityState.Deleted;
        }
        else
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                Delete(entity);
            }
        }
    }

    #endregion[END_CRUD_OPERATIONS]

    #region[QUERY_OPERATIONS]      


    #endregion[END_QUERY_OPERATIONS]

    protected override IQueryable<TEntity> SetTracking(IQueryable<TEntity> query, TrackingBehaviour tracking)
    {
        if (tracking == TrackingBehaviour.AsTracking)
        {
            query = query.AsTracking();
        }
        return query;
    }
}