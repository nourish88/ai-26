using Juga.Abstractions.Client;
using Serilog.Core;
using Serilog.Events;
namespace Juga.Logging.Serilog.Enrichers;

public class UserContextEnricher(IUserContextProvider userContextProvider) : ILogEventEnricher//log alanlarını zenginleştirici
{
   

    private const string ClientIdPropertyName = "ClientId";
    private const string ClientNamePropertyName = "ClientName";
    private const string UserCodePropertyName = "UserCode";
    private const string CorporateUserPropertyName = "CorporateUser";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (userContextProvider == null)
        {
            return;
        }
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(ClientIdPropertyName, userContextProvider?.ClientId));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(ClientNamePropertyName, userContextProvider?.ClientName));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(UserCodePropertyName, userContextProvider?.UserCode));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(CorporateUserPropertyName, userContextProvider?.CorporateUser));
    }
}