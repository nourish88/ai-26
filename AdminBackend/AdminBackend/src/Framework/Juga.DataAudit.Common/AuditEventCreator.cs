using System.Collections.Concurrent;
using System.Reflection;
using Juga.Abstractions.Client;
using Juga.Abstractions.Data.AuditLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Juga.DataAudit.Common;

public class AuditEventCreator : IAuditEventCreator
{
    private static readonly ConcurrentDictionary<Type, bool> EntityAuditableState = new();
    private static readonly ConcurrentDictionary<Type, HashSet<string>> IgnoredTypeProperties = new();
    private readonly List<AuditEntityEntry> TrackingEntities = new();


    public Func<IEnumerable<AuditEvent>> CreateAuditEvents(DbContext dbContext, IUserContextProvider clientInfoProvider,
        DateTime eventTime)
    {
        SetTrackedEntities(dbContext);
        return () => GetAuditEvents(dbContext, clientInfoProvider, eventTime);
    }

    public void SetChanges(DbContext dbContext)
    {
        SetTrackedEntities(dbContext);
    }

    public IEnumerable<AuditEvent> GetEvents(DbContext dbContext, IUserContextProvider clientInfoProvider, DateTime eventTime)
    {
        return GetAuditEvents(dbContext, clientInfoProvider, eventTime);
    }

    private IEnumerable<AuditEvent> GetAuditEvents(DbContext dbContext, IUserContextProvider clientInfoProvider,
        DateTime eventTime)
    {
        var events = new List<AuditEvent>();
        var modifiedEntries = GetTrackedEntities();
        if (modifiedEntries.Count == 0) return events;
        foreach (var entry in modifiedEntries)
        {
            var primaryKeyProperties = entry.EntityEntry.Metadata.FindPrimaryKey()?.Properties;
            var entityMetaData = GetEntityMetaData(dbContext, entry.EntityEntry);

            if (primaryKeyProperties == null)
                throw new InvalidOperationException("Entity does not have a primary key.");


            var pkIsNumeric = primaryKeyProperties[0].ClrType == typeof(long) ||
                              primaryKeyProperties[0].ClrType == typeof(int) ||
                              primaryKeyProperties[0].ClrType == typeof(short) ||
                              primaryKeyProperties[0].ClrType == typeof(float) ||
                              primaryKeyProperties[0].ClrType == typeof(double) ||
                              primaryKeyProperties[0].ClrType == typeof(byte);
            object pkGuid = null;
            long? pk1Val = null;
            long? pk2Val = null;

            if (pkIsNumeric)
            {
                var currentValuesType = entry.EntityEntry.CurrentValues.GetType();
                var getValueMethod = currentValuesType.GetMethod("GetValue", new[] { typeof(IProperty) });

                if (getValueMethod != null)
                {
                    var currentValuesInstance = entry.EntityEntry.CurrentValues;

                    pk1Val = GetPkVal(getValueMethod, primaryKeyProperties[0], currentValuesInstance);
                    if (primaryKeyProperties.Count > 1)
                        pk2Val = GetPkVal(getValueMethod, primaryKeyProperties[1], currentValuesInstance);
                }
            }
            else if (primaryKeyProperties[0].ClrType == typeof(Guid))
            {
                pkGuid = entry.EntityEntry.CurrentValues.GetValue<Guid>(primaryKeyProperties[0]);
            }

            else
            {
                throw new NotSupportedException("Primary key type is neither long nor GUID.");
            }


            events.Add(new AuditEvent
            {
                EventTime = eventTime,
                EventType = entry.EventType,
                PkValue1 = pk1Val,
                PkValue2 = pk2Val,
                PkGuid = (Guid?)pkGuid,
                DatabaseName = entityMetaData.DatabaseName,
                SchemaName = entityMetaData.SchemaName,
                TableName = entityMetaData.TableName,
                User = clientInfoProvider.ClientId,
                PropertyValues = GetAuditablePropertyValues(dbContext, entry.EntityEntry)
            });
        }

        ClearTrackedEntities();
        return events;
    }

    private static long? GetPkVal(MethodInfo getValueMethod, IProperty pkProps1, PropertyValues currentValuesInstance)
    {
        var pk = getValueMethod.MakeGenericMethod(pkProps1.ClrType)
            .Invoke(currentValuesInstance, new object[] { pkProps1 });
        return Convert.ToInt64(pk);
    }


    private void SetTrackedEntities(DbContext dbContext)
    {
        dbContext.ChangeTracker.DetectChanges();
        TrackingEntities.AddRange(
            dbContext.ChangeTracker.Entries()
                .Where(x => x.State != EntityState.Unchanged
                            && x.State != EntityState.Detached
                            && IsEntityAuditable(x.Entity))
                .Select(entityEntry => new AuditEntityEntry
                { EntityEntry = entityEntry, EventType = EntityStateToAuditEvent(entityEntry.State) }).ToList()
        );
    }

    private List<AuditEntityEntry> GetTrackedEntities()
    {
        return TrackingEntities;
    }

    private void ClearTrackedEntities()
    {
        TrackingEntities.Clear();
    }

    private static bool IsEntityAuditable(object entity)
    {
        var type = entity.GetType();
        if (!EntityAuditableState.ContainsKey(type))
        {
            var ignoreAttribute = type.GetTypeInfo().GetCustomAttribute(typeof(AuditLogIgnoreAttribute), true);
            EntityAuditableState[type] = ignoreAttribute != null ? false : true;
        }

        return EntityAuditableState[type];
    }

    private static string EntityStateToAuditEvent(EntityState entityState)
    {
        switch (entityState)
        {
            case EntityState.Added:
                return AuditEventType.Insert;
            case EntityState.Modified:
                return AuditEventType.Update;
            case EntityState.Deleted:
                return AuditEventType.Delete;
            default:
                return AuditEventType.Unknown;
        }
    }

    private static EntityMetaData GetEntityMetaData(DbContext dbContext, EntityEntry entityEntry)
    {
        var result = new EntityMetaData();
        var definingType = dbContext.Model.FindEntityType(entityEntry.Entity.GetType());
        if (definingType == null) return result;
        var dbConnection = IsRelational(dbContext) ? dbContext.Database.GetDbConnection() : null;
        result.DatabaseName = dbConnection?.Database;
        result.TableName = definingType.GetTableName();
        result.SchemaName = definingType.GetSchema() ?? "dbo";
        return result;
    }

    private static Dictionary<string, object> GetAuditablePropertyValues(DbContext dbContext, EntityEntry entityEntry)
    {
        var result = new Dictionary<string, object>();
        var properties = entityEntry.Metadata.GetProperties();
        foreach (var prop in properties)
        {
            var propEntry = entityEntry.Property(prop.Name);
            if (IsPropertyAuditable(dbContext, entityEntry, prop.Name))
                result.Add(prop.GetColumnName(), propEntry.CurrentValue);
        }

        return result;
    }

    private static bool IsPropertyAuditable(DbContext dbContext, EntityEntry entityEntry, string propertyName)
    {
        var entityType = entityEntry.Metadata.ClrType;
        if (entityType == null) return true;
        var ignoredProperties = GetAuditIgnoredPropertiesOfType(entityType);
        if (ignoredProperties != null && ignoredProperties.Contains(propertyName)) return false;

        return true;
    }

    private static HashSet<string> GetAuditIgnoredPropertiesOfType(Type type)
    {
        if (!IgnoredTypeProperties.ContainsKey(type))
        {
            var ignoredProps = new HashSet<string>();
            foreach (var prop in type.GetTypeInfo().GetProperties())
            {
                var ignoreAttr = prop.GetCustomAttribute(typeof(AuditLogIgnorePropertyAttribute), true);
                if (ignoreAttr != null) ignoredProps.Add(prop.Name);
            }

            if (ignoredProps.Count > 0)
                IgnoredTypeProperties[type] = ignoredProps;
            else
                IgnoredTypeProperties[type] = null;
        }

        return IgnoredTypeProperties[type];
    }

    private static bool IsRelational(DbContext dbContext)
    {
        var provider = (IInfrastructure<IServiceProvider>)dbContext.Database;
        var relationalConnection = provider.Instance.GetService<IRelationalConnection>();
        return relationalConnection != null;
    }
}