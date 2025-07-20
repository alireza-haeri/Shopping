using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Features.Product.Commands;

public record DeleteProductCommand(Guid Id)
    :IRequest<OperationResult<bool>>,IValidatableModel<DeleteProductCommand>
{
    public IValidator<DeleteProductCommand> Validator(ValidationModelBase<DeleteProductCommand> validator)
    {
        validator.RuleFor(x=>x.Id).NotEmpty().WithMessage("Id is required");
        
        return validator;
    }
}