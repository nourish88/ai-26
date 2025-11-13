namespace Juga.Api.Helpers;

public static class CommonHelpers
{
    public static bool IsMediatrEnabled(IConfiguration configuration, IApiOptions options)
    {
        var k = configuration.GetValue<bool?>("Juga:CQRS:Enabled") == true;
        bool t = false;
        if (options != null)
        {
            t = options?.Mediator == true;
        }


        var isEnabled = k || t;

        return isEnabled;
    }
}