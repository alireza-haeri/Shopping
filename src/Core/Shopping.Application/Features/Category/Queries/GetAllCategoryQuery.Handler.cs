using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Repositories.Common;

namespace Shopping.Application.Features.Category.Queries;

public class GetAllCategoryQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllCategoryQuery, OperationResult<List<GetAllCategoryQueryResult>>>
{
    public async ValueTask<OperationResult<List<GetAllCategoryQueryResult>>> Handle(GetAllCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await unitOfWork.CategoryRepository.GetAllAsync(cancellationToken);
        if (categories.Count == 0)
            return OperationResult<List<GetAllCategoryQueryResult>>.SuccessResult(Enumerable
                .Empty<GetAllCategoryQueryResult>().ToList());

        return OperationResult<List<GetAllCategoryQueryResult>>.SuccessResult(
            categories
                .Select(c => new GetAllCategoryQueryResult(c.Id, c.Title, c.ParentId))
                .ToList());
    }
}