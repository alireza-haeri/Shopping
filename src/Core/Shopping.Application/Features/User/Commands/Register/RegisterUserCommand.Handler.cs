using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Contracts.User;
using Shopping.Application.Extensions;
using Shopping.Domain.Entities.User;

namespace Shopping.Application.Features.User.Commands.Register;

public class RegisterUserCommandHandler(IUserManager userManager)
    : IRequestHandler<RegisterUserCommand, OperationResult<bool>>
{
    public async ValueTask<OperationResult<bool>> Handle(RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = new UserEntity(request.FirstName, request.LastName, request.UserName, request.Email)
        {
            PhoneNumber = request.PhoneNumber
        };

        var userCreateResult = await userManager.PasswordCreateAsync(user, request.Password, cancellationToken);
        if (userCreateResult.Succeeded)
            return OperationResult<bool>.SuccessResult(true); //todo Send confirmation email code

        return OperationResult<bool>.FailureResult(userCreateResult.Errors.ConvertToKeyValuePairs());
    }
}