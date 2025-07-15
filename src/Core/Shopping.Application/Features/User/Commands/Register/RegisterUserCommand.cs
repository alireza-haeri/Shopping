using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Features.User.Commands.Register;

public record RegisterUserCommand(
    string FirstName,
    string? LastName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password,
    string RepeatPassword)
    : IRequest<OperationResult<bool>> , IValidatableModel<RegisterUserCommand>
{
    public IValidator<RegisterUserCommand> Validator(ValidationModelBase<RegisterUserCommand> validator)
    {
        validator.RuleFor(c => c.Email).NotEmpty().EmailAddress();
        validator.RuleFor(c => c.Password).NotEmpty();
        validator.RuleFor(c => c.RepeatPassword).NotEmpty().Equal(c=>c.Password);
        validator.RuleFor(c=>c.FirstName).NotEmpty().MaximumLength(100);
        validator.RuleFor(c=>c.LastName).MaximumLength(100);
        validator.RuleFor(c => c.UserName).NotEmpty().MaximumLength(100);
        validator.RuleFor(c => c.PhoneNumber).NotEmpty().Length(11).Matches("^[0-9]{11}$");
        
        return validator;
    }
}