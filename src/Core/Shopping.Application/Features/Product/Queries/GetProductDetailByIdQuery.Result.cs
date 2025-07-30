using AutoMapper;
using Shopping.Application.Common.MappingConfiguration;
using Shopping.Domain.Entities.Product;

namespace Shopping.Application.Features.Product.Queries;

public record GetProductDetailByIdQueryResult(
    Guid ProductId,
    string Title,
    string Description,
    decimal Price,
    int Quantity,
    ProductEntity.ProductState State,
    string? CategoryTitle,
    Guid? CategoryId,
    Guid OwnerId,
    string OwnerFirstName,
    string? OwnerLastName,
    string? OwnerPhone,
    string? OwnerEmail,
    string? OwnerUserName)
    : ICreateApplicationMapper<ProductEntity>
{
    public record ProductDetailImageModel(string ImageName, string ImageUrl);

    public ProductDetailImageModel[] ProductImages { get; set; }

    public void Map(Profile profile)
    {
        profile.CreateMap<ProductEntity, GetProductDetailByIdQueryResult>()
            .ConstructUsing(src => new GetProductDetailByIdQueryResult(
                src.Id,
                src.Title,
                src.Description,
                src.Price,
                src.Quantity,
                src.State,
                src.Category != null ? src.Category.Title : null,
                src.Category != null ? (Guid?)src.Category.Id : null,
                src.User.Id,
                src.User.FirstName,
                src.User.LastName,
                src.User.PhoneNumber,
                src.User.Email,
                src.User.UserName
            ))
            .ReverseMap();
    
    }
};