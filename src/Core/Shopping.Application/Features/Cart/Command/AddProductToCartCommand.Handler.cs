using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Contracts.User;
using Shopping.Application.Repositories.Common;
using Shopping.Domain.Entities.Cart;

namespace Shopping.Application.Features.Cart.Command;

public class AddProductToCartCommandHandler(IUnitOfWork unitOfWork, IUserManager userManager)
    : IRequestHandler<AddProductToCartCommand, OperationResult<bool>>
{
    public async ValueTask<OperationResult<bool>> Handle(AddProductToCartCommand request,
        CancellationToken cancellationToken)
    {
        var userExist = await userManager.ExistsAsync(request.UserId, cancellationToken);
        if (!userExist)
            return OperationResult<bool>.FailureResult(nameof(AddProductToCartCommand.UserId), "User not found");

        var product = await unitOfWork.ProductRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return OperationResult<bool>.FailureResult(nameof(AddProductToCartCommand.ProductId), "Product not found");

        if (product.Quantity < request.Quantity)
            return OperationResult<bool>.FailureResult(nameof(AddProductToCartCommand.Quantity),
                "The requested quantity is not available in stock.");

        var cart = await unitOfWork.CartRepository.GetCartByUserIdWithTrackingAsync(request.UserId, cancellationToken);
        if (cart is null)
        {
            cart = CartEntity.Create(request.UserId);
            await unitOfWork.CartRepository.CreateCartAsync(cart, cancellationToken);
        }

        cart.AddItem(request.ProductId, request.Quantity, product.Price);

        await unitOfWork.CommitAsync(cancellationToken);
        return OperationResult<bool>.SuccessResult(true);
    }
}