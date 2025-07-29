using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shopping.WebFramework.Models;

namespace Shopping.WebFramework.Filters;

public class OkResultAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        switch (context.Result)
        {
            case OkObjectResult okObjectResult:
            {
                var apiResult = new ApiResult<object>(true, ApiResultStatusCode.Ok.ToString("G"),
                    ApiResultStatusCode.Ok, okObjectResult.Value!);
                context.Result = new JsonResult(apiResult) { StatusCode = StatusCodes.Status200OK };
                break;
            }
            case OkResult okResult:
            {
                var apiResult = new ApiResult(true, ApiResultStatusCode.Ok.ToString("G"),
                    ApiResultStatusCode.Ok);
                context.Result = new JsonResult(apiResult) { StatusCode = StatusCodes.Status200OK };
                break;
            }
            default: return;
        }
    }
}