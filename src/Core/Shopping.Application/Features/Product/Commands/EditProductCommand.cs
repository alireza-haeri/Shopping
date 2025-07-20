using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;
using Shopping.Domain.Entities.Product;

namespace Shopping.Application.Features.Product.Commands;

public record EditProductCommand(
    Guid ProductId,
    string? Title,
    string? Description,
    decimal? Price,
    int? Quantity,
    ProductEntity.ProductState? State,
    Guid? CategoryId,
    string[]? RemovedImages,
    List<EditProductCommand.AddedImagesContent>? AddedImages)
    : IRequest<OperationResult<bool>>, IValidatableModel<EditProductCommand>
{
    public record AddedImagesContent(string Base64File, string FileContent);

    public IValidator<EditProductCommand> Validator(ValidationModelBase<EditProductCommand> validator)
    {
        validator.RuleFor(x => x.CategoryId).NotEmpty().When(c => c.CategoryId != null);
        validator.RuleFor(x => x.Title).MaximumLength(100);
        validator.RuleFor(x => x.Description).MaximumLength(1000);
        validator.RuleFor(x => x.Price).GreaterThan(0).When(p => p.Price != null);
        validator.RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).When(q => q.Quantity != null);
        validator.RuleFor(x => x.State).IsInEnum().When(s => s.State != null);
        validator.RuleFor(x => x.AddedImages)
            .Must(images => images == null || images.All(i =>
                !string.IsNullOrWhiteSpace(i.Base64File) &&
                !string.IsNullOrWhiteSpace(i.FileContent)));

        return validator;
    }
}