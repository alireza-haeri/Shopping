using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Contracts.User.Models;

namespace Shopping.Application.Features.User.Queries.PasswordLogin;

public record UserPasswordLoginQuery(string UserNameOrEmail, string Password)
    : IRequest<OperationResult<JwtAccessTokenModel>>;