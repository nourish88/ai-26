using Juga.Mvc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Juga.Mvc;

public class BaseController : Controller
{
    protected async Task<T> CallApiAsync<T>(Func<Task<T>> asyncFunc)
    {
        try
        {
            return await asyncFunc.Invoke();
        }
        catch (Exception apiException)
        {
            HandleApiException(apiException);
            return default;
        }
    }

    protected T CallApi<T>(Func<T> func)
    {
        try
        {
            return func.Invoke();
        }
        catch (Exception ex)
        {
            var handled = HandleApiException(ex);
            if (!handled) throw;
            return default;
        }
    }

    private bool HandleApiException(Exception ex)
    {
        var statusCodeProperty = ex.GetType().GetProperty("StatusCode");
        var responseProperty = ex.GetType().GetProperty("Response");
        if (statusCodeProperty == null || responseProperty == null) return false;
        var statusCode = (int?)statusCodeProperty.GetValue(ex);
        var response = (string)responseProperty.GetValue(ex);

        if (statusCode.HasValue && statusCode.Value == StatusCodes.Status400BadRequest &&
            !string.IsNullOrWhiteSpace(response))
        {
            var apiError = JsonConvert.DeserializeObject<ApiValidationError>(response);
            if (apiError != null && apiError.errors.Count > 0)
                foreach (var error in apiError.errors)
                foreach (var errorValue in error.Value)
                    ModelState.AddModelError(error.Key, errorValue);
            return true;
        }

        return false;
    }
}