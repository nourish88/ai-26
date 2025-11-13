using Juga.Abstractions.Logging;
using Juga.Logging.Serilog.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Juga.Mvc.Extensions;

public static class MvcApplicationRunnerExtensions
{
    private static LoggingOptions _loggingOptions;
    public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    private static LoggingOptions LoggingOptions
    {
        get
        {
            if (_loggingOptions == null)
            {
                var loggingOptions = new LoggingOptions();
                Configuration.Bind(LoggingOptions.OptionsSection, loggingOptions);
                _loggingOptions = loggingOptions;
            }

            return _loggingOptions;
        }
    }

    public static void RunHost(Func<string[], IHostBuilder> builderFunc, string[] args)
    {
        var loggingOptions = LoggingOptions;
        switch (loggingOptions.LoggerType)
        {
            case LoggerType.Microsoft:
            {
                WithMicorostLogging(builderFunc, args);
                break;
            }
            case LoggerType.Serilog:
            {
                SerilogApplicationRunnerExtensions.RunHost(builderFunc, args, loggingOptions);
                break;
            }
            default:
            {
                throw new Exception("Not supported LoggerType");
            }
        }
    }

    private static void WithMicorostLogging(Func<string[], IHostBuilder> builderFunc, string[] args)
    {
        builderFunc.Invoke(args).Build().Run();
    }
}