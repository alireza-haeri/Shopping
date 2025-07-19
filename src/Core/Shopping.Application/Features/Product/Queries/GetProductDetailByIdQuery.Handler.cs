using AutoMapper;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Contracts.FileService.Interfaces;
using Shopping.Application.Repositories.Common;
using Shopping.Domain.Entities.Product;

namespace Shopping.Application.Features.Product.Queries;

public class GetProductDetailByIdQueryHandler(IUnitOfWork unitOfWork, IFileService fileService, IMapper mapper)
    : IRequestHandler<GetProductDetailByIdQuery, OperationResult<GetProductDetailByIdQueryResult>>
{
    public async ValueTask<OperationResult<GetProductDetailByIdQueryResult>> Handle(GetProductDetailByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await unitOfWork.ProductRepository.GetDetailsByIdAsync(request.Id, cancellationToken);
        if (product == null)
            return OperationResult<GetProductDetailByIdQueryResult>.NotFoundResult(nameof(GetProductDetailByIdQuery.Id),
                "Product not found");

        var productImages = await fileService
            .GetFilesByNameAsync(product.Images.Select(i => i.FileName).ToList(), cancellationToken);

        var result = mapper.Map<ProductEntity, GetProductDetailByIdQueryResult>(product);
        result.ProductImages = productImages
            .Select(i => new GetProductDetailByIdQueryResult.ProductDetailImageModel(i.FileName, i.FileUrl)).ToArray();

        return OperationResult<GetProductDetailByIdQueryResult>.SuccessResult(result);
    }
}