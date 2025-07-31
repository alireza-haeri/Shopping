using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Features.Cart.Command;

public record UpdateCartItemQuantityCommand(Guid UserId, Guid ProductId, int NewQuantity)
    :IRequest<OperationResult<bool>>,IValidatableModel<UpdateCartItemQuantityCommand>
{
    public IValidator<UpdateCartItemQuantityCommand> Validator(ValidationModelBase<UpdateCartItemQuantityCommand> validator)
    {
        validator.RuleFor(c => c.UserId)
            .NotEmpty();

        validator.RuleFor(c => c.ProductId)
            .NotEmpty();
            
        validator.RuleFor(c => c.NewQuantity)
            .GreaterThan(0);

        return validator;
    }
}