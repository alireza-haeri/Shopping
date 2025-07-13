using FluentValidation;

namespace Shopping.Application.Common.Validation;

public interface IValidatableModel <TRequestApplicationModel> where TRequestApplicationModel : class
{
    IValidator<TRequestApplicationModel> Validator(ValidationModelBase<TRequestApplicationModel> validator);
}