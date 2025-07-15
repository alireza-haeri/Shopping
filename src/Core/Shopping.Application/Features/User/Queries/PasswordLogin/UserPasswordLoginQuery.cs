using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;
using Shopping.Application.Contracts.User.Models;

namespace Shopping.Application.Features.User.Queries.PasswordLogin;

public record UserPasswordLoginQuery(string UserNameOrEmail, string Password)
    : IRequest<OperationResult<JwtAccessTokenModel>>
        , IValidatableModel<UserPasswordLoginQuery>
{
    public IValidator<UserPasswordLoginQuery> Validator(ValidationModelBase<UserPasswordLoginQuery> validator)
    {
        validator.RuleFor(c => c.UserNameOrEmail).NotEmpty().MaximumLength(100);
        validator.RuleFor(c => c.Password).NotEmpty().MaximumLength(100);

        return validator;
    }
}