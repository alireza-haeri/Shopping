using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Contracts.FileService.Interfaces;
using Shopping.Application.Contracts.FileService.Models;
using Shopping.Application.Repositories.Common;
using Shopping.Domain.Common.ValueObjects;

namespace Shopping.Application.Features.Product.Commands;

public class EditProductCommandHandler(IUnitOfWork unitOfWork, IFileService fileService)
    : IRequestHandler<EditProductCommand, OperationResult<bool>>
{
    public async ValueTask<OperationResult<bool>> Handle(EditProductCommand request,
        CancellationToken cancellationToken)
    {
        if (request.CategoryId.HasValue)
        {
            var category =
                await unitOfWork.CategoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category is null)
                return OperationResult<bool>.FailureResult(nameof(EditProductCommand.CategoryId), "Category not found");
        }

        var product = await unitOfWork.ProductRepository.GetByIdAsTrackAsync(request.ProductId, cancellationToken);
        if (product is null)
            return OperationResult<bool>.FailureResult(nameof(EditProductCommand.ProductId), "Product not found");

        try
        {
            product.Edit(
                request.Title,
                request.Description,
                request.Price,
                request.Quantity,
                request.CategoryId);

            if (request.State.HasValue)
                product.ChangeState(request.State.Value);
        }
        catch (Exception e)
        {
            return OperationResult<bool>.DomainFailureResult(e.Message);
        }

        if (request.RemovedImages?.Length != 0)
        {
            await fileService.RemoveFileAsync(request.RemovedImages, cancellationToken);
            product.RemoveImages(request.RemovedImages);
        }

        if (request.AddedImages?.Count != 0)
        {
            var savedFiles = await fileService.SaveFilesAsync(
                request.AddedImages
                    .Select(i => new SaveFileModel(i.Base64File, i.FileContent)).ToList(),
                cancellationToken);

            foreach (var file in savedFiles)
                product.AddImage(new ImageValueObject(file.FileName, file.FileType,
                    string.Empty));
        }

        await unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<bool>.SuccessResult(true);
    }
}