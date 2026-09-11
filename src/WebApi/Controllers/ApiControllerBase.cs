using CleanArchitecture.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApi.Controllers;

[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleFailure(Result result) => result.Error.Code switch
    {
        "not_found" => Problem(statusCode: StatusCodes.Status404NotFound, title: "Resource not found", detail: result.Error.Message),
        "conflict" => Problem(statusCode: StatusCodes.Status409Conflict, title: "Conflict", detail: result.Error.Message),
        _ => Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid request", detail: result.Error.Message)
    };
}
