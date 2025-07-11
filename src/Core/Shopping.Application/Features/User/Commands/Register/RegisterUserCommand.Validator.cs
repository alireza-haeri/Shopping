using FluentValidation;

namespace Shopping.Application.Features.User.Commands.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Password).NotEmpty();
        RuleFor(c => c.RepeatPassword).NotEmpty().Equal(c=>c.Password);
        RuleFor(c=>c.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(c=>c.LastName).MaximumLength(100);
        RuleFor(c => c.UserName).NotEmpty().MaximumLength(100);
        RuleFor(c => c.PhoneNumber).NotEmpty().Length(11);
    }
}