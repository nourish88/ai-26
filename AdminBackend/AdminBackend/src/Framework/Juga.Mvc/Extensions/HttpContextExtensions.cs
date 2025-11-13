using Microsoft.AspNetCore.Http;

namespace Juga.Mvc.Extensions;

public static class HttpContextExtensions
{
    public static bool IsAjaxRequest(this HttpRequest httpRequest)
    {
        return httpRequest.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}