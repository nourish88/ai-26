using AdminBackend.Application.Services.App;
using Serilog.Core;
using Serilog.Events;

namespace AdminBackend.Infrastructure.Logging
{
    public class AppContextEnricher(IAppService appService) : ILogEventEnricher
    {
        private const string AppIdentifierPropertyName = "ApplicationIdentifier";


        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (appService == null || string.IsNullOrEmpty(appService.RequesterApplicationIdentifier))
            {
                return;
            }
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(AppIdentifierPropertyName, appService.RequesterApplicationIdentifier));
        }
    }
}
