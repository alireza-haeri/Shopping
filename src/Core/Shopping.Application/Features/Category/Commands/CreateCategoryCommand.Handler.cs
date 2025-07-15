using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Repositories.Common;
using Shopping.Domain.Entities.Product;

namespace Shopping.Application.Features.Category.Commands;

public class CreateCategoryCommandHandler
(IUnitOfWork unitOfWork)
:IRequestHandler<CreateCategoryCommand,OperationResult<bool>>
{
    public async ValueTask<OperationResult<bool>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new CategoryEntity(request.Title, request.ParentId);
        await unitOfWork.CategoryRepository.CreateAsync(category, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<bool>.SuccessResult(true);
    }
}