using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Features.Category.Queries;

public record GetByIdCategoryQuery(Guid Id)
    : IRequest<OperationResult<GetByIdCategoryQueryResult>>, IValidatableModel<GetByIdCategoryQuery>
{
    public IValidator<GetByIdCategoryQuery> Validator(ValidationModelBase<GetByIdCategoryQuery> validator)
    {
        validator.RuleFor(c => c.Id).NotEmpty().NotNull();

        return validator;
    }
}