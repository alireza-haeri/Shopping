using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Extensions;

namespace Shopping.Application.Features.Common;

public class ValidateRequestBehavior<TRequest, TResponse>(IValidator<TRequest> validator)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IOperationResult , new ()
{
    public async ValueTask<TResponse> Handle(TRequest message, CancellationToken cancellationToken,
        MessageHandlerDelegate<TRequest, TResponse> next)
    {
        var validationResult = await validator.ValidateAsync(message, cancellationToken);
        if (!validationResult.IsValid)
            return new TResponse()
            {
                IsSuccess = false,
                ErrorMessages = validationResult.Errors.ConvertKeyValuePairs()
            };
        
        return await next(message, cancellationToken);
    }
}