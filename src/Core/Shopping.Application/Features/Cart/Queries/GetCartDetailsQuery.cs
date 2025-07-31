using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Features.Cart.Queries;

public record GetCartDetailsQuery(Guid UserId)
    : IRequest<OperationResult<GetCartDetailsQueryResult?>>, IValidatableModel<GetCartDetailsQuery>
{
    public IValidator<GetCartDetailsQuery> Validator(ValidationModelBase<GetCartDetailsQuery> validator)
    {
        validator.RuleFor(x => x.UserId)
            .NotNull()
            .WithMessage("UserId is required");

        return validator;
    }
}