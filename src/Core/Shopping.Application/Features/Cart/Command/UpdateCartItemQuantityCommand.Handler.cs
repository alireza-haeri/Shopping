using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Contracts.User;
using Shopping.Application.Repositories.Common;
using System.Linq; // Add this using directive

namespace Shopping.Application.Features.Cart.Command;

public class UpdateCartItemQuantityCommandHandler(IUnitOfWork unitOfWork, IUserManager userManager)
    : IRequestHandler<UpdateCartItemQuantityCommand, OperationResult<bool>>
{
    public async ValueTask<OperationResult<bool>> Handle(UpdateCartItemQuantityCommand request,
        CancellationToken cancellationToken)
    {
        var cart = await unitOfWork.CartRepository.GetCartByUserIdWithTrackingAsync(request.UserId, cancellationToken);
        if (cart is null)
            return OperationResult<bool>.FailureResult(nameof(UpdateCartItemQuantityCommand.UserId),
                "Cart not found for this user.");


        var itemToUpdate = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        if (itemToUpdate is null)
            return OperationResult<bool>.FailureResult(nameof(UpdateCartItemQuantityCommand.ProductId),
                "Product does not exist in the cart.");


        var product = await unitOfWork.ProductRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return OperationResult<bool>.FailureResult(nameof(UpdateCartItemQuantityCommand.ProductId),
                "Product not found in catalog.");

        if (product.Quantity < request.NewQuantity)
            return OperationResult<bool>.FailureResult(nameof(UpdateCartItemQuantityCommand.NewQuantity),
                "The requested quantity is not available in stock.");


        cart.UpdateItemQuantity(itemToUpdate.Id, request.NewQuantity);

        await unitOfWork.CommitAsync(cancellationToken);
        return OperationResult<bool>.SuccessResult(true);
    }
}