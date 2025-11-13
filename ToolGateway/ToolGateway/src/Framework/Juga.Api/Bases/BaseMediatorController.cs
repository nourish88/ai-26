using ResultType = Juga.Abstractions.Application.Models.ResultType;

namespace Juga.Api.Bases;

[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class BaseMediatorController : ControllerBase
{
    public IActionResult ReturnByResultType<T>(Result<T> result)
    {
        if (result.ResultType == ResultType.Unauthorized) return Unauthorized(result);
        if (result.ResultType != ResultType.Ok)
        {
            var statusCode = result.ResultType == ResultType.Unexpected ? (int)HttpStatusCode.InternalServerError :
                result.ResultType == ResultType.Invalid ? (int)HttpStatusCode.BadRequest :
                result.ResultType == ResultType.NotFound ? (int)HttpStatusCode.NotFound :
                result.ResultType == ResultType.Unauthorized ? (int)HttpStatusCode.Unauthorized : (int)HttpStatusCode.InternalServerError;

            return BadRequest(new ProblemDetails
            { Title = result.Messages?.FirstOrDefault() ?? "Kayıt esnasında hata oluştu", Status = statusCode });
        }

        return Ok(result);
    }
}