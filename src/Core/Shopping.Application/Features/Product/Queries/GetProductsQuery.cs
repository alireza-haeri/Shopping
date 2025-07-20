using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Features.Product.Queries;

public record GetProductsQuery(string Title, int CurrentPage, int PageCount, Guid? CategoryId = null)
    :IRequest<OperationResult<List<GetProductsQueryResult>>>,IValidatableModel<GetProductsQuery>
{
    public IValidator<GetProductsQuery> Validator(ValidationModelBase<GetProductsQuery> validator)
    {
        validator.RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must be at most 100 characters.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters.");

        validator.RuleFor(x => x.CurrentPage)
            .GreaterThan(0).WithMessage("CurrentPage must be greater than 0.");

        validator.RuleFor(x => x.PageCount)
            .GreaterThan(0).WithMessage("PageCount must be greater than 0.");

        return validator;
    }
}