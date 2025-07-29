using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shopping.Application.Common;
using Shopping.WebFramework.Extensions;

namespace Shopping.WebFramework.Common;

public class BaseController : ControllerBase
{
    protected string? UserName => base.User.Identity?.Name;

    protected Guid? UserId => Guid.Parse(User.Identity?.GetUserId()!);

    protected string? UserEmail => User.Identity?.FindFirstValue(ClaimTypes.Email);

    protected string? UserKey => User.Identity?.FindFirstValue(ClaimTypes.UserData);


    protected IActionResult OperationResult<TModel>(OperationResult<TModel> result)
    {
        if (result.IsSuccess)
            return result.Result is bool ? Ok() : Ok(result.Result);

        if (result.IsNotFound)
        {
            AddErrors(result);

            var errors = new ValidationProblemDetails(ModelState);

            return NotFound(errors.Errors);
        }

        AddErrors(result);

        var badRequestErrors = new ValidationProblemDetails(ModelState);

        return BadRequest(badRequestErrors);
    }

    private void AddErrors<TModel>(OperationResult<TModel> result)
    {
        if (result.ErrorMessages is null) return;
        foreach (var errorMessage in result.ErrorMessages)
        {
            ModelState.AddModelError(errorMessage.Key, errorMessage.Value);
        }
    }
}