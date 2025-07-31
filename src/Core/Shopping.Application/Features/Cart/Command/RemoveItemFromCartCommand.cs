using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Features.Cart.Command;

public record RemoveItemFromCartCommand(Guid UserId, Guid ProductId)
    : IRequest<OperationResult<bool>>,IValidatableModel<RemoveItemFromCartCommand>
{
    public IValidator<RemoveItemFromCartCommand> Validator(ValidationModelBase<RemoveItemFromCartCommand> validator)
    {
        validator.RuleFor(c => c.UserId)
            .NotEmpty();

        validator.RuleFor(c => c.ProductId)
            .NotEmpty();

        return validator;
    }
}