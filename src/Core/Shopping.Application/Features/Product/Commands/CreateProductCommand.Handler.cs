using Mediator;
using Microsoft.AspNetCore.Identity;
using Shopping.Application.Common;
using Shopping.Application.Contracts.FileService.Interfaces;
using Shopping.Application.Contracts.FileService.Models;
using Shopping.Application.Contracts.User;
using Shopping.Application.Repositories.Common;
using Shopping.Domain.Common.ValueObjects;
using Shopping.Domain.Entities.Product;

namespace Shopping.Application.Features.Product.Commands;

public class CreateProductCommandHandler(IUnitOfWork unitOfWork, IFileService fileService, IUserManager userManager)
    : IRequestHandler<CreateProductCommand, OperationResult<bool>>
{
    public async ValueTask<OperationResult<bool>> Handle(CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        if (request.CategoryId.HasValue)
        {
            var category = await unitOfWork.CategoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category is null)
                return OperationResult<bool>.FailureResult(nameof(CreateProductCommand.CategoryId), "Category not found");
        }

        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return OperationResult<bool>.FailureResult(nameof(CreateProductCommand.UserId), "User not found");
        ProductEntity product;

        try
        {
            product = ProductEntity.Create(
                request.Title,
                request.Description,
                request.Price,
                request.Quantity,
                request.State,
                request.UserId,
                request.CategoryId);
        }
        catch (Exception e)
        {
            return OperationResult<bool>.DomainFailureResult(e.Message);
        }

        if (request.Images?.Length > 0)
        {
            var savedImages = await fileService.SaveFilesAsync(
                request.Images.Select(i => new SaveFileModel(i.Base64File, i.FileContent)).ToList(), cancellationToken);
            savedImages.ForEach(i => 
                product.AddImage(new ImageValueObject(i.FileName, i.FileType, string.Empty)));
        }

        await unitOfWork.ProductRepository.CreateAsync(product, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<bool>.SuccessResult(true);
    }
}