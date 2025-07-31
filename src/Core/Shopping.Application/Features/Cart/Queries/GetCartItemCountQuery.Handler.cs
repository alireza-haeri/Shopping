using Mediator;
using Microsoft.EntityFrameworkCore;
using Shopping.Application.Common;
using Shopping.Application.Contracts;

namespace Shopping.Application.Features.Cart.Queries;

public class GetCartItemCountQueryHandler(IReadDbContext dbContext)
    : IRequestHandler<GetCartItemCountQuery, OperationResult<int>>
{
    public async ValueTask<OperationResult<int>> Handle(GetCartItemCountQuery request, CancellationToken cancellationToken)
    {
        var count = await dbContext.Carts
            .AsNoTracking()
            .Where(cart => cart.UserId == request.UserId)
            .SelectMany(cart => cart.Items) 
            .SumAsync(item => item.Quantity, cancellationToken);

        return OperationResult<int>.SuccessResult(count);
    }
}