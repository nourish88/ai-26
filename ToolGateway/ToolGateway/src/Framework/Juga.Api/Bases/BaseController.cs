using ResultType = Juga.Abstractions.Application.Models.ResultType;

namespace Juga.Api.Bases;

using System.Linq;

/// <summary>
/// Temel Api Controller Sınıfı
/// </summary>
[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class BaseController : ControllerBase
{
    public IActionResult ActionResultMaker<T>(Result<T> kimlikResult)
    {
        if (kimlikResult.ResultType == ResultType.Ok)

            return Ok(kimlikResult);

        if (kimlikResult.ResultType == ResultType.NotFound)

            return NotFound();

        if (kimlikResult.ResultType == ResultType.Invalid)
            return BadRequest();
        if (kimlikResult.ResultType == ResultType.Unauthorized)
            return Unauthorized();
        return Ok(kimlikResult);
    }

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