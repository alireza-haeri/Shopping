using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Contracts.FileService.Interfaces;
using Shopping.Application.Repositories.Common;
using Shopping.Domain.Common.ValueObjects;

namespace Shopping.Application.Features.Product.Queries;

public class GetProductsQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
    : IRequestHandler<GetProductsQuery, OperationResult<List<GetProductsQueryResult>>>
{
    public async ValueTask<OperationResult<List<GetProductsQueryResult>>> Handle(GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.CategoryId.HasValue)
        {
            var category =
                await unitOfWork.CategoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category is null)
                return OperationResult<List<GetProductsQueryResult>>.FailureResult(nameof(GetProductsQuery.CategoryId),
                    "Category not found");
        }

        var products = await unitOfWork.ProductRepository.GetProductsAsync(request.Title, request.CurrentPage,
            request.PageCount, request.CategoryId, cancellationToken);

        var results = new List<GetProductsQueryResult>();

        foreach (var product in products)
        {
            GetProductsQueryResult result = new(product.Id, product.Title, product.Price, product.Quantity);
            if (product.Images.Any())
            {
                var file = (await fileService.GetFilesByNameAsync([product.Images[0].FileName], cancellationToken))
                    .FirstOrDefault();
                if (file is not null)
                    result.ProductImage = new GetProductsQueryResult.ProductImageModel(file!.FileName, file.FileUrl);
            }

            results.Add(result);
        }

        return OperationResult<List<GetProductsQueryResult>>.SuccessResult(results);
    }
}