using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;
using Shopping.Domain.Entities.Product;

namespace Shopping.Application.Features.Product.Commands;

public record CreateProductCommand(
    Guid UserId,
    Guid? CategoryId,
    string Title,
    string Description,
    decimal Price,
    int Quantity,
    ProductEntity.ProductState State,
    CreateProductCommand.CreateProductImagesModel[]? Images)
:IRequest<OperationResult<bool>>,IValidatableModel<CreateProductCommand>
{
    public record CreateProductImagesModel(string Base64File, string FileContent);

    public IValidator<CreateProductCommand> Validator(ValidationModelBase<CreateProductCommand> validator)
    {
        validator.RuleFor(x => x.UserId).NotEmpty();
        validator.RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        validator.RuleFor(x => x.Description).MaximumLength(1000);
        validator.RuleFor(x => x.Price).GreaterThan(0);
        validator.RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        validator.RuleFor(x => x.State).IsInEnum();
        validator.RuleFor(x => x.Images)
            .Must(images => images == null || images.All(i =>
                !string.IsNullOrWhiteSpace(i.Base64File) &&
                !string.IsNullOrWhiteSpace(i.FileContent)));

        return validator;
    }

}