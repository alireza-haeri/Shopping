using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Repositories.Common;

namespace Shopping.Application.Features.Category.Queries;

public class GetByIdCategoryQueryHandler
(IUnitOfWork unitOfWork)
:IRequestHandler<GetByIdCategoryQuery,OperationResult<GetByIdCategoryQueryResult>>
{
    public async ValueTask<OperationResult<GetByIdCategoryQueryResult>> Handle(GetByIdCategoryQuery request, CancellationToken cancellationToken)
    {
        var category = await unitOfWork.CategoryRepository.GetByIdAsync(request.Id,cancellationToken);
        if(category == null)
            return OperationResult<GetByIdCategoryQueryResult>.NotFoundResult(nameof(GetByIdCategoryQuery.Id),"Category not found");
        
        return OperationResult<GetByIdCategoryQueryResult>.SuccessResult(new GetByIdCategoryQueryResult(category.Title, category.ParentId));
    }
}