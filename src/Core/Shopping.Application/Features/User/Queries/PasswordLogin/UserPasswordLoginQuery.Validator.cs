using FluentValidation;

namespace Shopping.Application.Features.User.Queries.PasswordLogin;

public class UserPasswordLoginQueryValidator : AbstractValidator<UserPasswordLoginQuery>
{
    public UserPasswordLoginQueryValidator()
    {
        RuleFor(c => c.UserNameOrEmail).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Password).NotEmpty().MaximumLength(100);
    }
}