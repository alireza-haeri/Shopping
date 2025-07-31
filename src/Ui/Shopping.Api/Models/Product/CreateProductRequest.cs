using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;
using Shopping.Application.Features.Product.Commands;
using Shopping.Domain.Entities.Product;

namespace Shopping.Api.Models.Product;

public record CreateProductRequest(
    Guid? CategoryId,
    string Title,
    string Description,
    decimal Price,
    int Quantity,
    ProductEntity.ProductState State,
    CreateProductCommand.CreateProductImagesModel[]? Images)
{
    public record CreateProductImagesModel(string Base64File, string FileContent);
}