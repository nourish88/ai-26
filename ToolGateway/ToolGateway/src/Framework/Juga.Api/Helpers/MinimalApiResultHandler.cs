
namespace Juga.Api.Helpers;

public class MinimalApiResultHandler
{

    public static IResult Problem(List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Results.Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    private static IResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Results.Problem(
            statusCode: statusCode,
            title: error.Description,
            detail: error.Code,
            type: statusCode switch
            {
                404 => "Not Found Error",
                409 => "Conflict Error",
                400 => "Validation Error",
                403 => "Forbidden Error",
                500 => "Internal Server Error",
                _ => "Undefined Error"
            });
    }

    private static IResult ValidationProblem(List<Error> errors)
    {
        var problemDetails = new ValidationProblemDetails();

        errors.ForEach(error => problemDetails.Errors.Add(error.Code, new[] { error.Description }));

        return Results.ValidationProblem(problemDetails.Errors);
    }
}


