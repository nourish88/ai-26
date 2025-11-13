
using ResultType = Juga.Abstractions.Application.Models.ResultType;

namespace Juga.Api.Bases;

[ApiController]
[Produces("application/json")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]

public class BaseCleanArcApiController : ControllerBase
{
  

    protected ActionResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    private ObjectResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Problem(statusCode: statusCode, title: error.Description, detail: error.Code, type: statusCode switch
        {
            404 => "Not Found Error",
            409 => "Conflict Error",
            400 => "Validation Error",
            403 => "Forbidden Error",
            500 => "Internal Server Error",
            _ => "Undefined Error"
        });
    }

    private ActionResult ValidationProblem(List<Error> errors)
    {
        var modelStateDictionary = new ModelStateDictionary();

        errors.ForEach(error => modelStateDictionary.AddModelError(error.Code, error.Description));

        return ValidationProblem(modelStateDictionary);
    }
}