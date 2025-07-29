using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shopping.WebFramework.Models;

namespace Shopping.WebFramework.Filters;

public class BadRequestAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is not BadRequestObjectResult badRequestObjectResult)
            return;

        var modelState = context.ModelState;

        if (!modelState.IsValid)
        {
            var errors = new ValidationProblemDetails(modelState);
            var validationErrorApiResult = new ApiResult<IDictionary<string, string[]>>(false,
                ApiResultStatusCode.BadRequest.ToString("G"), ApiResultStatusCode.BadRequest,
                errors.Errors);

            context.Result = new JsonResult(validationErrorApiResult) { StatusCode = StatusCodes.Status400BadRequest };
            return;
        }

        var badRequestApiResult = new ApiResult<object>(false, ApiResultStatusCode.BadRequest.ToString("G"),
            ApiResultStatusCode.BadRequest,badRequestObjectResult.Value!);
        
        context.Result = new JsonResult(badRequestApiResult) { StatusCode = StatusCodes.Status400BadRequest };
    }
}