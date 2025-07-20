using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Shopping.Application.Common;
using Shopping.Application.Features.Common;

namespace Shopping.Application.Test;

public static class Helpers
{
    public static ValueTask<TResponse> ValidateAndExecuteAsync<TRequest, TResponse>
    (TRequest request, IRequestHandler<TRequest, TResponse> handler, IServiceProvider serviceProvider)
    where TRequest : IRequest<TResponse> where TResponse: IOperationResult , new ()
    {
        var validator = serviceProvider.GetRequiredService<IValidator<TRequest>>();
        var validateRequestBehavior = new ValidateRequestBehavior<TRequest, TResponse>(validator);
        return validateRequestBehavior.Handle(request, CancellationToken.None, handler.Handle);
    }
}