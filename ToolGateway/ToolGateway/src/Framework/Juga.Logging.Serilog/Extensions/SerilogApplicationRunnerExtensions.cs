using System.Diagnostics;
using Juga.Abstractions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.MSSqlServer;
using LogLevel = Juga.Abstractions.Logging.LogLevel;
using RollingInterval = Juga.Abstractions.Logging.RollingInterval;
using Ser = Serilog;

namespace Juga.Logging.Serilog.Extensions;

public static class SerilogApplicationRunnerExtensions
{
    /// <summary>
    ///     /// Runtime'da log level'ı switch etmek için kullanılır.
    ///     /// &lt;/summary&gt;
    public static readonly LoggingLevelSwitch Switcher = new();

    /// /// &lt;summary&gt;
    /// Hosting ile ilgili hataları yakalamak için kullanılır.
    /// Juga.Api-> Program.cs
    /// </summary>
    /// <param name="builderFunc"></param>
    /// <param name="args"></param>
    /// <param name="loggingOptions"></param>
    public static void RunHost(Func<string[], IHostBuilder> builderFunc, string[] args, LoggingOptions loggingOptions)
    {
        Log.Logger = CreateLogger(loggingOptions);
        try
        {
            Log.Information("Application starting.");
            var hostBuilder = builderFunc.Invoke(args).UseSerilog();
            if (loggingOptions.ClearDefaultLogProviders)
                hostBuilder.ConfigureLogging((hostingContext, config) => { config.ClearProviders(); });
            hostBuilder.Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    ///     Uygulamadaki hataları yakalamak için, tanımlı konfigürasyonu kullanarak başlatır.
    /// </summary>
    /// <param name="loggingOptions"></param>
    /// <returns></returns>
    private static Ser.ILogger CreateLogger(LoggingOptions loggingOptions)
    {
        if (loggingOptions.EnableSelfLog)
        {
            SelfLog.Enable(msg => Debug.WriteLine(msg));
            SelfLog.Enable(Console.Error);
        }

        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", loggingOptions.ApplicationName);

        if (loggingOptions.OverrideMicrosoftLevels)
            loggerConfiguration.MinimumLevel
                .Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning);

        foreach (var context in loggingOptions.ExcludedSourceContext)
            loggerConfiguration.Filter.ByExcluding(Matching.FromSource(context));

        foreach (var overrideLogLevel in loggingOptions.OverrideLevels)
            loggerConfiguration.MinimumLevel.Override(overrideLogLevel.Key,
                LogLevelToLogEventLevel(overrideLogLevel.Value));

        if (loggingOptions.EnableWriteToConsole) loggerConfiguration.WriteTo.Console();

        if (loggingOptions.EnableWriteToDebug) loggerConfiguration.WriteTo.Debug();

        if (loggingOptions.EnableWriteToFile)
        {
            if (loggingOptions.LogToFileOptions == null)
                throw new Exception("Log to file options are missing for Write to file");
            loggerConfiguration.WriteTo.File(
                new CompactJsonFormatter(),
                loggingOptions.LogToFileOptions.Path,
                LogLevelToLogEventLevel(loggingOptions.LogToFileOptions.MinimumLevel),
                loggingOptions.LogToFileOptions.FileSizeLimitBytes,
                rollingInterval: ToSerilogInterval(loggingOptions.LogToFileOptions.RollingInterval)
            );
        }

        if (loggingOptions.EnableWriteToElasticSearch)
        {
            if (loggingOptions.LogToElasticSearchOptions == null)
                throw new Exception("Log to elasticsearch options are missing for Write to ElasticSearch");
            loggerConfiguration.WriteTo.Elasticsearch(
                new ElasticsearchSinkOptions(new Uri(loggingOptions.LogToElasticSearchOptions.Uri))
                {
                    MinimumLogEventLevel =
                        LogLevelToLogEventLevel(loggingOptions.LogToElasticSearchOptions.MinimumLevel),
                    AutoRegisterTemplate = loggingOptions.LogToElasticSearchOptions.AutoRegisterTemplate,
                    IndexFormat = loggingOptions.LogToElasticSearchOptions.IndexFormat,
                    TemplateName = loggingOptions.LogToElasticSearchOptions.TemplateName,
                    CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true)
                });
        }

        if (loggingOptions.EnableWriteToMsSqlServer)
        {
            if (loggingOptions.LogToMsSqlServerOptions == null)
                throw new Exception("Log to MsSqlServer options are missing for Write to MSSQL");
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
                new MSSqlServerSinkOptions
                {
                    AutoCreateSqlTable = loggingOptions.LogToMsSqlServerOptions.AutoCreateSqlTable,
                    SchemaName = loggingOptions.LogToMsSqlServerOptions.SchemaName,
                    TableName = loggingOptions.LogToMsSqlServerOptions.TableName,
                    BatchPeriod = TimeSpan.FromSeconds(loggingOptions.LogToMsSqlServerOptions.BatchPeriod),
                    BatchPostingLimit = loggingOptions.LogToMsSqlServerOptions.BatchPostLimit
                },
                restrictedToMinimumLevel: LogLevelToLogEventLevel(loggingOptions.LogToMsSqlServerOptions.MinimumLevel),
                columnOptions: columnOptions
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

        return loggerConfiguration.CreateLogger();
    }

    private static LogEventLevel LogLevelToLogEventLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Fatal => LogEventLevel.Fatal,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Verbose => LogEventLevel.Verbose,
            LogLevel.Warning => LogEventLevel.Warning,
            _ => LogEventLevel.Information
        };
    }

    private static Ser.RollingInterval ToSerilogInterval(RollingInterval rollingInterval)
    {
        return rollingInterval switch
        {
            RollingInterval.None => Ser.RollingInterval.Infinite,
            RollingInterval.Year => Ser.RollingInterval.Year,
            RollingInterval.Month => Ser.RollingInterval.Month,
            RollingInterval.Day => Ser.RollingInterval.Day,
            RollingInterval.Hour => Ser.RollingInterval.Hour,
            RollingInterval.Minute => Ser.RollingInterval.Minute,
            _ => Ser.RollingInterval.Day
        };
    }
}