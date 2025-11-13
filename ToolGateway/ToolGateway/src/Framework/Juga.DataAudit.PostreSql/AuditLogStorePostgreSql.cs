using Juga.Abstractions.Data.AuditLog;
using Juga.DataAudit.Common;
using Juga.DataAudit.PostreSql.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Npgsql;

namespace Juga.DataAudit.PostreSql;

public class AuditLogStorePostgreSql : IAuditLogStore
{
    private readonly DataAuditPostgreSqlOptions _dataAuditPostgreSqlOptions;
    private readonly string _insertCommandColumnNames;
    private readonly string _insertCommandValues;
    private readonly string _insertCommand;
    public AuditLogStorePostgreSql(IOptions<DataAuditPostgreSqlOptions> dataAuditPostgreSqlOptions)
    {
        _dataAuditPostgreSqlOptions = dataAuditPostgreSqlOptions.Value;
        _insertCommandColumnNames = CreateInsertColumnNames();
        _insertCommandValues = CreateInsertCommandValues();
        _insertCommand = CreateInsertCommand();
    }

    public void StoreAuditEvents(IEnumerable<AuditEvent> auditEvents)
    {
        StoreAuditEventsInternal(auditEvents);
    }
    private void StoreAuditEventsInternal(IEnumerable<AuditEvent> auditEvents)
    {
        using var context = CreateAuditContext();
        foreach (var auditEvent in auditEvents)
        {
            var sqlParameters = GetSqlParametersForInsert(auditEvent);
            context.Database.ExecuteSqlRaw(_insertCommand, sqlParameters);
        }
    }
    private string CreateInsertColumnNames()
    {
        List<string> columnNames = new() {
            _dataAuditPostgreSqlOptions.AuditTableColumnNameForEventTime,
            _dataAuditPostgreSqlOptions.AuditTableColumnNameForEventType,
            _dataAuditPostgreSqlOptions.AuditTableColumnNameForPk1,
            _dataAuditPostgreSqlOptions.AuditTableColumnNameForPkGuid,
            _dataAuditPostgreSqlOptions.AuditTableColumnNameForPk2,
            _dataAuditPostgreSqlOptions.AuditTableColumnNameForPropertyValues,
            _dataAuditPostgreSqlOptions.AuditTableColumnNameForDatabaseName,
            _dataAuditPostgreSqlOptions.AuditTableColumnNameForSchemaName,
            _dataAuditPostgreSqlOptions.AuditTableColumnNameForTableName,
            _dataAuditPostgreSqlOptions.AuditTableColumnNameForUser
        };
        return string.Join(", ", columnNames.Select(c => $"\"{c}\""));
    }
    private string CreateInsertCommandValues()
    {
        List<string> values = new() {
            ParameterNames.EventTime,
            ParameterNames.EventType,
            ParameterNames.PkValue1,
            ParameterNames.PkGuid,
            ParameterNames.PkValue2,
            ParameterNames.PropertyValues,
            ParameterNames.DatabaseName,
            ParameterNames.SchemaName,
            ParameterNames.TableName,
            ParameterNames.User
        };
        return string.Join(", ", values.Select(c => c));
    }
    private string CreateInsertCommand()
    {
        return $"INSERT INTO {_dataAuditPostgreSqlOptions.SchemaName}.\"{_dataAuditPostgreSqlOptions.TableName}\" ({_insertCommandColumnNames}) VALUES ({_insertCommandValues})";
    }
    private AuditContext CreateAuditContext()
    {
        return new AuditContext(_dataAuditPostgreSqlOptions.ConnectionString);
    }
    private NpgsqlParameter[] GetSqlParametersForInsert(AuditEvent auditEvent)
    {
        var parameters = new List<NpgsqlParameter>
        {
            new NpgsqlParameter(ParameterNames.EventTime, auditEvent.EventTime),
            new NpgsqlParameter(ParameterNames.EventType, auditEvent.EventType),
            new NpgsqlParameter(ParameterNames.PkValue1, auditEvent.PkValue1.HasValue ? auditEvent.PkValue1.Value : DBNull.Value),
            new NpgsqlParameter(ParameterNames.PkGuid, auditEvent.PkGuid.HasValue ? auditEvent.PkGuid.Value : DBNull.Value),
            new NpgsqlParameter(ParameterNames.PkValue2, auditEvent.PkValue2.HasValue ? auditEvent.PkValue2.Value : DBNull.Value),
            new NpgsqlParameter(ParameterNames.PropertyValues, ToJson(auditEvent.PropertyValues)),
            new NpgsqlParameter(ParameterNames.DatabaseName, auditEvent.DatabaseName),
            new NpgsqlParameter(ParameterNames.SchemaName, string.IsNullOrWhiteSpace(auditEvent.SchemaName) ? "public" : auditEvent.SchemaName),
            new NpgsqlParameter(ParameterNames.TableName, auditEvent.TableName),
            new NpgsqlParameter(ParameterNames.User, string.IsNullOrWhiteSpace(auditEvent.User) ? DBNull.Value : auditEvent.User)
        };
        return parameters.ToArray();
    }
    private string ToJson(Dictionary<string, object> propertyValues)
    {
        return JsonConvert.SerializeObject(propertyValues);
    }
}