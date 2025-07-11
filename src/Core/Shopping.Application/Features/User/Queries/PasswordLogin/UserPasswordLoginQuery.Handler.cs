using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Contracts.User;
using Shopping.Application.Contracts.User.Models;
using Shopping.Application.Extensions;

namespace Shopping.Application.Features.User.Queries.PasswordLogin;

public class UserPasswordLoginQueryHandler(IUserManager userManager,IJwtService jwtService)
    : IRequestHandler<UserPasswordLoginQuery, OperationResult<JwtAccessTokenModel>>
{
    public async ValueTask<OperationResult<JwtAccessTokenModel>> Handle(UserPasswordLoginQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await new UserPasswordLoginQueryValidator().ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return OperationResult<JwtAccessTokenModel>.FailureResult(validationResult.Errors.ConvertKeyValuePairs());
        
        var user = request.UserNameOrEmail.IsEmail()
            ? await userManager.FindByEmailAsync(request.UserNameOrEmail, cancellationToken)
            : await userManager.FindByUserNameAsync(request.UserNameOrEmail, cancellationToken);
        
        if(user is null)
            return OperationResult<JwtAccessTokenModel>.NotFoundResult(nameof(request.UserNameOrEmail),"User not found");

        var passwordValidation = await userManager.PasswordCreateAsync(user, request.Password, cancellationToken);
        if (passwordValidation.Succeeded)
        {
            var accessToken = await jwtService.GenerateJwtTokenAsync(user,cancellationToken);
            return OperationResult<JwtAccessTokenModel>.SuccessResult(accessToken);
        }
        
        return OperationResult<JwtAccessTokenModel>.FailureResult(passwordValidation.Errors.ConvertToKeyValuePairs());
    }
}