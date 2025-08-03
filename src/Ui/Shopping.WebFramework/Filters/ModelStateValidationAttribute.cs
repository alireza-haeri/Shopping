using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Shopping.WebFramework.Models;

namespace Shopping.WebFramework.Filters;

public class ModelStateValidationAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var actionArgument in context.ActionArguments.Values)
        {
            var validator = context.HttpContext
                .RequestServices.GetService(typeof(IValidator<>)
                    .MakeGenericType(actionArgument!.GetType()));

            if (validator is IValidator validatorInstance)
            {
                var validationResult =
                    await validatorInstance.ValidateAsync(new ValidationContext<object>(actionArgument));

                if (!validationResult.IsValid)
                    validationResult.Errors.ForEach(e =>
                        context.ModelState.AddModelError(e.PropertyName, e.ErrorMessage));
            }
        }

        var modelState = context.ModelState;

        if (!modelState.IsValid)
        {
            var errors = new ValidationProblemDetails(modelState);

            var apiResult =
                new ApiResult<IDictionary<string, string[]>>(false, ApiResultStatusCode.BadRequest.ToString("G"),
                    ApiResultStatusCode.BadRequest,
                    errors.Errors);
            context.Result = new JsonResult(apiResult) { StatusCode = StatusCodes.Status400BadRequest };
        }

        await base.OnActionExecutionAsync(context, next);
    }
}