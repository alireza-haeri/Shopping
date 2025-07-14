using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Features.Category.Commands;

public record CreateCategoryCommand(string Title, Guid? ParentId = null)
    :IRequest<OperationResult<bool>>,IValidatableModel<CreateCategoryCommand>
{
    public IValidator<CreateCategoryCommand> Validator(ValidationModelBase<CreateCategoryCommand> validator)
    {
        validator.RuleFor(c=>c.Title).NotNull().NotEmpty().MaximumLength(100);

        return validator;
    }
}