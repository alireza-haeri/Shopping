using Mediator;
using Shopping.Application.Common;

namespace Shopping.Application.Features.User.Commands.Register;

public record RegisterUserCommand(
    string FirstName,
    string? LastName,
    string UserName,
    string Email,
    string PhoneNumber,
    string Password,
    string RepeatPassword)
    : IRequest<OperationResult<bool>>;