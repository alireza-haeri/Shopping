using FluentValidation;

namespace Shopping.Application.Common.Validation;

/// <summary>
/// Marker Validation Class
/// </summary>
/// <typeparam name="TRequestModel">A request model in form of command or query</typeparam>
public class ValidationModelBase<TRequestModel> : AbstractValidator<TRequestModel> where TRequestModel : class
{
    
}