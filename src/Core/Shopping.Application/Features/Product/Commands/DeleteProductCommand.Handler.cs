using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Contracts.FileService.Interfaces;
using Shopping.Application.Repositories.Common;

namespace Shopping.Application.Features.Product.Commands;

public class DeleteProductCommandHandler(IUnitOfWork unitOfWork, IFileService fileService)
    : IRequestHandler<DeleteProductCommand, OperationResult<bool>>
{
    public async ValueTask<OperationResult<bool>> Handle(DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await unitOfWork.ProductRepository.GetByIdAsTrackAsync(request.Id, cancellationToken);
        if (product is null)
            return OperationResult<bool>.NotFoundResult(nameof(DeleteProductCommand.Id), "Product not found");


        await unitOfWork.ProductRepository.DeleteAsync(product, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        await fileService.RemoveFileAsync(
            product.Images.Select(i => i.FileName).ToArray(), cancellationToken);
        
        return OperationResult<bool>.SuccessResult(true);
    }
}