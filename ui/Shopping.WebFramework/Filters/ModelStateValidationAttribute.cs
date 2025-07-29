using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shopping.WebFramework.Filters;

public class ModelStateValidationAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var actionArguments in context.ActionArguments.Values)
        {
            var validator = context.HttpContext.RequestServices.GetService(typeof(IValidator<>)
                .MakeGenericType(actionArguments!.GetType()));

            if (validator is IValidator validatorInstance)
            {
                var validationResult = await validatorInstance.ValidateAsync(new ValidationContext<object>(actionArguments));
                if(!validationResult.IsValid)
                    validationResult.Errors.ForEach(e=>context.ModelState.AddModelError(e.PropertyName,e.ErrorMessage));
            }
        }
        
        await base.OnActionExecutionAsync(context, next);
    }
}