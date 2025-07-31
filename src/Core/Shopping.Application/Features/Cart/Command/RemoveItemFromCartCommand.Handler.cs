using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Repositories.Common;

namespace Shopping.Application.Features.Cart.Command;

public class RemoveItemFromCartCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveItemFromCartCommand, OperationResult<bool>>
{
    public async ValueTask<OperationResult<bool>> Handle(RemoveItemFromCartCommand request,
        CancellationToken cancellationToken)
    {
        var cart = await unitOfWork.CartRepository.GetCartByUserIdWithTrackingAsync(request.UserId, cancellationToken);
        if (cart is null)
            return OperationResult<bool>.FailureResult(nameof(RemoveItemFromCartCommand.UserId),
                "Cart not found for this user.");
        
        var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        if (itemToRemove is null)
            return OperationResult<bool>.FailureResult(nameof(RemoveItemFromCartCommand.ProductId),
                "Product does not exist in the cart.");

        cart.RemoveItem(itemToRemove.Id);

        await unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<bool>.SuccessResult(true);
    }
}