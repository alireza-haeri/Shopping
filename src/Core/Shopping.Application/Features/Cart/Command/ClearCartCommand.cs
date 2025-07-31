using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Features.Cart.Command;

public record ClearCartCommand(Guid UserId)
    : IRequest<OperationResult<bool>>, IValidatableModel<ClearCartCommand>
{
    public IValidator<ClearCartCommand> Validator(ValidationModelBase<ClearCartCommand> validator)
    {
        validator.RuleFor(c => c.UserId)
            .NotEmpty();

        return validator;
    }
}