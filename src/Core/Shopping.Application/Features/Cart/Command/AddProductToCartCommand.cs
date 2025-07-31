using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Features.Cart.Command;

public record AddProductToCartCommand(Guid UserId, Guid ProductId, int Quantity)
    : IRequest<OperationResult<bool>>, IValidatableModel<AddProductToCartCommand>
{
    public IValidator<AddProductToCartCommand> Validator(ValidationModelBase<AddProductToCartCommand> validator)
    {
        validator.RuleFor(c => c.UserId)
            .NotEmpty()
            .WithMessage("User identifier is required.");

        validator.RuleFor(c => c.ProductId)
            .NotEmpty()
            .WithMessage("Product identifier is required.");

        validator.RuleFor(c => c.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        return validator;
    }
}