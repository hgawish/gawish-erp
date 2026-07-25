using GawishERP.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace GawishERP.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok();

        return result.Error.Type switch
        {
            ErrorType.Validation => BadRequest(result.Error),

            ErrorType.NotFound => NotFound(result.Error),

            ErrorType.Conflict => Conflict(result.Error),

            ErrorType.Unauthorized => Unauthorized(result.Error),

            ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result.Error),

            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);

        return result.Error.Type switch
        {
            ErrorType.Validation => BadRequest(result.Error),

            ErrorType.NotFound => NotFound(result.Error),

            ErrorType.Conflict => Conflict(result.Error),

            ErrorType.Unauthorized => Unauthorized(result.Error),

            ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result.Error),

            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }
}