using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Repositories.Common;

namespace Shopping.Application.Features.Cart.Command;

public class ClearCartCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ClearCartCommand, OperationResult<bool>>
{
    public async ValueTask<OperationResult<bool>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await unitOfWork.CartRepository.GetCartByUserIdWithTrackingAsync(request.UserId, cancellationToken);
        if (cart is null) return OperationResult<bool>.SuccessResult(true);
        
        cart.Clear();
        await unitOfWork.CommitAsync(cancellationToken);
        
        return OperationResult<bool>.SuccessResult(true);
    }
}