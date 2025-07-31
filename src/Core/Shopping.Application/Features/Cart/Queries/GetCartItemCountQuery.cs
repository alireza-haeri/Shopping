using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Features.Cart.Queries;

public record GetCartItemCountQuery(Guid UserId)
    : IRequest<OperationResult<int>>, IValidatableModel<GetCartItemCountQuery>
{
    public IValidator<GetCartItemCountQuery> Validator(ValidationModelBase<GetCartItemCountQuery> validator)
    {
        validator.RuleFor(x => x.UserId).NotEmpty();
        return validator;
    }
}