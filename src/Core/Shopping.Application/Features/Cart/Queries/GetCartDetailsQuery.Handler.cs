using Mediator;
using Microsoft.EntityFrameworkCore;
using Shopping.Application.Common;
using System.Linq;
using Shopping.Application.Contracts;

namespace Shopping.Application.Features.Cart.Queries;

public class GetCartDetailsQueryHandler(IReadDbContext dbContext)
    : IRequestHandler<GetCartDetailsQuery, OperationResult<GetCartDetailsQueryResult?>>
{
    public async ValueTask<OperationResult<GetCartDetailsQueryResult?>> Handle(GetCartDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var queryResult = await dbContext.Carts
            .AsNoTracking()
            .Where(cart => cart.UserId == request.UserId)
            .Select(cart => new GetCartDetailsQueryResult(
                cart.Id,
                cart.Items.Select(item => new GetCartDetailsQueryResult.CartItem(
                    item.Id,
                    item.ProductId,
                    item.Product.Title,
                    item.Product.Images.Select(img => img.FileName).FirstOrDefault(),
                    item.Quantity,
                    item.UnitPrice
                )).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return OperationResult<GetCartDetailsQueryResult?>.SuccessResult(queryResult);
    }
}