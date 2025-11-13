using Juga.Abstractions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ser = Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using ILogger = Serilog.ILogger;
using LogLevel = Juga.Abstractions.Logging.LogLevel;
using Serilog;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Serilog.Sinks.MSSqlServer;
using Serilog.Filters;
using Microsoft.Extensions.Configuration;
using NpgsqlTypes;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.PostgreSQL.ColumnWriters;
using Serilog.Core;
using Serilog.Sinks.OpenTelemetry;


namespace Juga.Logging.Serilog.Extensions;

public static class SerilogProgramRunnerExtensions
{   /// <summary>
    ///     /// Runtime'da log level'ı switch etmek için kullanılır.
    ///     /// &lt;/summary&gt;
    public static readonly LoggingLevelSwitch Switcher = new();
    /// <summary>
    /// Hosting ile ilgili hataları yakalamak için kullanılır.
    /// Juga.Api-> Program.cs
    /// </summary>
    /// <param name="builderFunc"></param>
    /// <param name="args"></param>
    /// <param name="loggingOptions"></param>
    public static void RunHost(WebApplicationBuilder builder)
    {


        var loggingOptions = new LoggingOptions();
        builder.Configuration.Bind(LoggingOptions.OptionsSection, loggingOptions);
        Log.Logger = CreateLogger(loggingOptions);
        try
        {
            Log.Information("Application starting.");
            var hostBuilder = builder.Host.UseSerilog();
            if (loggingOptions.ClearDefaultLogProviders)
            {
                hostBuilder.ConfigureLogging((hostingContext, config) => { config.ClearProviders(); });
            }
            //hostBuilder.Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        //finally
        //{
        //    Log.CloseAndFlush();
        //}
    }
    /// <summary>
    /// Uygulamadaki hataları yakalamak için, tanımlı konfigürasyonu kullanarak başlatır.
    /// </summary>
    /// <param name="loggingOptions"></param>
    /// <returns></returns>
    private static ILogger CreateLogger(LoggingOptions loggingOptions)
    {

        if (loggingOptions.EnableSelfLog)
        {
            Ser.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
            Ser.Debugging.SelfLog.Enable(Console.Error);
        }
       

        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", loggingOptions.ApplicationName);

        if (loggingOptions.OverrideMicrosoftLevels)
        {

            loggerConfiguration.MinimumLevel
                .Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning);
        }

        foreach (var context in loggingOptions.ExcludedSourceContext)
        {
            loggerConfiguration.Filter.ByExcluding(Matching.FromSource(context));
        }

        foreach (var overrideLogLevel in loggingOptions.OverrideLevels)
        {
            loggerConfiguration.MinimumLevel.Override(overrideLogLevel.Key, LogLevelToLogEventLevel(overrideLogLevel.Value));
        }

        if (loggingOptions.EnableWriteToConsole)
        {
            loggerConfiguration.WriteTo.Console();
        }

        if (loggingOptions.EnableWriteToDebug)
        {
            loggerConfiguration.WriteTo.Debug();
        }

        if (loggingOptions.EnableWriteToFile)
        {
            if (loggingOptions.LogToFileOptions == null)
            {
                throw new Exception("Log to file options are missing for Write to file");
            }
            loggerConfiguration.WriteTo.File(
                formatter: new CompactJsonFormatter(),
                path: loggingOptions.LogToFileOptions.Path,
                restrictedToMinimumLevel: LogLevelToLogEventLevel(loggingOptions.LogToFileOptions.MinimumLevel),
                fileSizeLimitBytes: loggingOptions.LogToFileOptions.FileSizeLimitBytes,
                rollingInterval: ToSerilogInterval(loggingOptions.LogToFileOptions.RollingInterval)
            );
        }

        if (loggingOptions.EnableWriteToElasticSearch)
        {
            if (loggingOptions.LogToElasticSearchOptions == null)
            {
                throw new Exception("Log to elasticsearch options are missing for Write to ElasticSearch");
            }
            loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(loggingOptions.LogToElasticSearchOptions.Uri))
            {
                MinimumLogEventLevel = LogLevelToLogEventLevel(loggingOptions.LogToElasticSearchOptions.MinimumLevel),
                AutoRegisterTemplate = loggingOptions.LogToElasticSearchOptions.AutoRegisterTemplate,
                IndexFormat = loggingOptions.LogToElasticSearchOptions.IndexFormat,
                TemplateName = loggingOptions.LogToElasticSearchOptions.TemplateName,
                CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),

            });
        }

        if (loggingOptions.EnableWriteToMsSqlServer)
        {
            if (loggingOptions.LogToMsSqlServerOptions == null)
            {
                throw new Exception("Log to MsSqlServer options are missing for Write to MSSQL");
            }
            var columnOptions = new ColumnOptions
            {
                //AdditionalColumns = new List<SqlColumn>
                //{
                //    new() { ColumnName = "ErrorBy", DataType = SqlDbType.NVarChar, DataLength = 50 },
                //    new() { ColumnName = "ErrorIp", DataType = SqlDbType.NVarChar, DataLength = 50 }
                //},
                Store = new HashSet<StandardColumn>
                {
                    StandardColumn.Message, // Include the Message column
                    StandardColumn.Level, // Include the Level column
                    StandardColumn.TimeStamp, // Include the TimeStamp column
                    StandardColumn.Exception, // Include the Exception column (if needed)
                    StandardColumn.Properties // Include the Properties column (if needed)
                    // Exclude StandardColumn.MessageTemplate
                    // Exclude StandardColumn.LogEvent
                }
            };

            loggerConfiguration.WriteTo.MSSqlServer(
                loggingOptions.LogToMsSqlServerOptions.ConnectionString,
                new MSSqlServerSinkOptions()
                {
                    AutoCreateSqlTable = loggingOptions.LogToMsSqlServerOptions.AutoCreateSqlTable,
                    SchemaName = loggingOptions.LogToMsSqlServerOptions.SchemaName,
                    TableName = loggingOptions.LogToMsSqlServerOptions.TableName,
                    BatchPeriod = TimeSpan.FromSeconds(loggingOptions.LogToMsSqlServerOptions.BatchPeriod),
                    BatchPostingLimit = loggingOptions.LogToMsSqlServerOptions.BatchPostLimit
                }, restrictedToMinimumLevel: LogLevelToLogEventLevel(loggingOptions.LogToMsSqlServerOptions.MinimumLevel),
                columnOptions: columnOptions
            );
        }

        if (loggingOptions.EnableWriteToPostgresql)
        {
            if (loggingOptions.LogToPostgresqlOptions == null)
            {
                throw new Exception("Log to Postgre options are missing for Write to Postgresql server");
            }

            IDictionary<string, ColumnWriterBase> columnOptions = new Dictionary<string, ColumnWriterBase>
            {
                { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
                
                { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
                { "timestamp", new TimestampColumnWriter() },
                { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
                { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) }
            };
            loggerConfiguration.WriteTo.PostgreSQL(
                loggingOptions.LogToPostgresqlOptions.ConnectionString,
                loggingOptions.LogToPostgresqlOptions.TableName,
                columnOptions,
                restrictedToMinimumLevel: LogLevelToLogEventLevel(
                    loggingOptions.LogToPostgresqlOptions.MinimumLevel),
                period: TimeSpan.FromSeconds(loggingOptions.LogToPostgresqlOptions.BatchPeriod),
                batchSizeLimit: loggingOptions.LogToPostgresqlOptions.BatchPostLimit,
                schemaName: loggingOptions.LogToPostgresqlOptions.SchemaName,
                needAutoCreateTable: loggingOptions.LogToPostgresqlOptions.AutoCreateTable,
                needAutoCreateSchema: loggingOptions.LogToPostgresqlOptions.AutoCreateSchema
            );
        }

        if (loggingOptions.EnableWriteToSeq)
        {
            if (loggingOptions.LogToSeqOptions == null)
                throw new Exception("Log to Seq options are missing for Write to Seq");
            loggerConfiguration
                .MinimumLevel.ControlledBy(Switcher)
                .WriteTo.Seq(loggingOptions.LogToSeqOptions.Uri,
                    LogLevelToLogEventLevel(loggingOptions.LogToSeqOptions.MinimumLevel));
        }

        if(loggingOptions.EnableWriteToOpenTelemetry)
        {
            if (loggingOptions.LogToOpenTelemetryOptions == null)
                throw new Exception("Log to Seq options are missing for Write to Seq");

            loggerConfiguration.WriteTo.OpenTelemetry(c =>
            {
                c.Endpoint = loggingOptions.LogToOpenTelemetryOptions.CollectorUrl;
                c.Protocol = OtlpProtocol.Grpc;
                c.IncludedData = IncludedData.TraceIdField | IncludedData.SpanIdField | IncludedData.SourceContextAttribute;
                c.ResourceAttributes = new Dictionary<string, object>
                                                        {
                                                        {"service.name", loggingOptions.ApplicationName},
                                                        {"index", 10},
                                                        {"flag", true},
                                                        {"value", 3.14}
                                                        };
            });
        }

        return loggerConfiguration.CreateLogger();
    }

    private static LogEventLevel LogLevelToLogEventLevel(LogLevel logLevel)
    {
        switch (logLevel)
        {
            case LogLevel.Debug:
            {
                return LogEventLevel.Debug;
            }
            case LogLevel.Error:
            {
                return LogEventLevel.Error;
            }
            case Abstractions.Logging.LogLevel.Fatal:
            {
                return LogEventLevel.Fatal;
            }
            case LogLevel.Information:
            {
                return LogEventLevel.Information;
            }
            case LogLevel.Verbose:
            {
                return LogEventLevel.Verbose;
            }
            case LogLevel.Warning:
            {
                return LogEventLevel.Warning;
            }
            default:
            {
                return LogEventLevel.Information;
            }
        }
    }

    private static Ser.RollingInterval ToSerilogInterval(Abstractions.Logging.RollingInterval rollingInterval)
    {
        return rollingInterval switch
        {
            Abstractions.Logging.RollingInterval.None => Ser.RollingInterval.Infinite,
            Abstractions.Logging.RollingInterval.Year => Ser.RollingInterval.Year,
            Abstractions.Logging.RollingInterval.Month => Ser.RollingInterval.Month,
            Abstractions.Logging.RollingInterval.Day => Ser.RollingInterval.Day,
            Abstractions.Logging.RollingInterval.Hour => Ser.RollingInterval.Hour,
            Abstractions.Logging.RollingInterval.Minute => Ser.RollingInterval.Minute,
            _ => Ser.RollingInterval.Day
        };
    }
}