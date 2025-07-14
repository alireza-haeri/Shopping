using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Features.Category.Queries;

public record GetAllCategoryQuery()
    :IRequest<OperationResult<List<GetAllCategoryQueryResult>>>,IValidatableModel<GetAllCategoryQuery>
{
    public IValidator<GetAllCategoryQuery> Validator(ValidationModelBase<GetAllCategoryQuery> validator)
    {
        return validator;
    }
}