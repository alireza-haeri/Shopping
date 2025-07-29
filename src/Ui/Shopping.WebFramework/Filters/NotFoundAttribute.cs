using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shopping.WebFramework.Models;

namespace Shopping.WebFramework.Filters;

public class NotFoundAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        switch (context.Result)
        {
            case NotFoundObjectResult notFoundObjectResult:
            {
                var apiResult = new ApiResult<object>(false, ApiResultStatusCode.NotFound.ToString("G"),
                    ApiResultStatusCode.NotFound,
                    notFoundObjectResult.Value!);

                context.Result = new JsonResult(apiResult) { StatusCode = StatusCodes.Status404NotFound };
                break;
            }
            case NotFoundResult notFoundResult:
            {
                var apiResult = new ApiResult(false, ApiResultStatusCode.NotFound.ToString("G"),
                    ApiResultStatusCode.NotFound);

                context.Result = new JsonResult(apiResult) { StatusCode = StatusCodes.Status404NotFound };
                break;
            }
        }
    }
}