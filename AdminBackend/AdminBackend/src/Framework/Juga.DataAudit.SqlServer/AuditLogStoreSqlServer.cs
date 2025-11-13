using System.Transactions;
using Juga.Abstractions.Data.AuditLog;
using Juga.DataAudit.SqlServer.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Juga.DataAudit.Common;
namespace Juga.DataAudit.SqlServer;

public class AuditLogStoreSqlServer : IAuditLogStore
{
    private readonly DataAuditSqlServerOptions _dataAuditSqlServerOptions;
    private readonly bool _guidAuditExists;
    private readonly string _insertCommand;
    private readonly string _insertCommandColumnNames;
    private readonly string _insertCommandValues;

    public AuditLogStoreSqlServer(IOptions<DataAuditSqlServerOptions> dataAuditSqlServerOptions,
        IConfiguration configuration)
    {
        _guidAuditExists = configuration.GetSection("Juga:DataAudit:SqlServer:AuditTableColumnNameForPkGuid").Value !=
                           null;
        _dataAuditSqlServerOptions = dataAuditSqlServerOptions.Value;
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
        var columnNames = new List<string>
        {
            _dataAuditSqlServerOptions.AuditTableColumnNameForEventTime,
            _dataAuditSqlServerOptions.AuditTableColumnNameForEventType,
            _dataAuditSqlServerOptions.AuditTableColumnNameForPk1,
            _dataAuditSqlServerOptions.AuditTableColumnNameForPk2,

            _dataAuditSqlServerOptions.AuditTableColumnNameForPropertyValues,
            _dataAuditSqlServerOptions.AuditTableColumnNameForDatabaseName,
            _dataAuditSqlServerOptions.AuditTableColumnNameForSchemaName,
            _dataAuditSqlServerOptions.AuditTableColumnNameForTableName,
            _dataAuditSqlServerOptions.AuditTableColumnNameForUser
        };


        //var a = _configuration.GetSection("Juga:DataAudit");
        //var guidExists = _configuration.GetValue<string>("Juga:DataAudit:TableName");


        if (_guidAuditExists)
            columnNames.Add(_dataAuditSqlServerOptions.AuditTableColumnNameForPkGuid);
        return string.Join(", ", columnNames.Select(c => $"[{c}]"));
    }

    private string CreateInsertCommandValues()
    {
        var values = new List<string>
        {
            ParameterNames.EventTime,
            ParameterNames.EventType,
            ParameterNames.PkValue1,
            ParameterNames.PkValue2,

            ParameterNames.PropertyValues,
            ParameterNames.DatabaseName,
            ParameterNames.SchemaName,
            ParameterNames.TableName,
            ParameterNames.User
        };

        if (_guidAuditExists)
            values.Add(ParameterNames.PkGuid);
        return string.Join(", ", values.Select(c => c));
    }

    private string CreateInsertCommand()
    {
        return
            $"INSERT INTO [{_dataAuditSqlServerOptions.SchemaName}].[{_dataAuditSqlServerOptions.TableName}] ({_insertCommandColumnNames}) VALUES ({_insertCommandValues})";
    }

    private AuditContext CreateAuditContext()
    {
        return new AuditContext(_dataAuditSqlServerOptions.ConnectionString);
    }

    private SqlParameter[] GetSqlParametersForInsert(AuditEvent auditEvent)
    {
        var parameters = new List<SqlParameter>
        {
            new(ParameterNames.EventTime, auditEvent.EventTime),
            new(ParameterNames.EventType, auditEvent.EventType),
            new(ParameterNames.PkValue1, auditEvent.PkValue1.HasValue ? auditEvent.PkValue1.Value : DBNull.Value),
            new(ParameterNames.PkValue2, auditEvent.PkValue2.HasValue ? auditEvent.PkValue2.Value : DBNull.Value),

            new(ParameterNames.PropertyValues, ToJson(auditEvent.PropertyValues)),
            new(ParameterNames.DatabaseName, auditEvent.DatabaseName),
            new(ParameterNames.SchemaName,
                string.IsNullOrWhiteSpace(auditEvent.SchemaName) ? "dbo" : auditEvent.SchemaName),
            new(ParameterNames.TableName, auditEvent.TableName),
            new(ParameterNames.User, string.IsNullOrWhiteSpace(auditEvent.User) ? DBNull.Value : auditEvent.User)
        };
        if (_guidAuditExists)
            parameters.Add(new SqlParameter(ParameterNames.PkGuid,
                auditEvent.PkGuid.HasValue ? auditEvent.PkGuid.Value : DBNull.Value));


        return parameters.ToArray();
    }

    private string ToJson(Dictionary<string, object> propertyValues)
    {
        return JsonConvert.SerializeObject(propertyValues);
    }
}