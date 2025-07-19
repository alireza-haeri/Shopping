using AutoMapper;
using FluentValidation;
using Mediator;
using Shopping.Application.Common;
using Shopping.Application.Common.MappingConfiguration;
using Shopping.Application.Common.Validation;
using Shopping.Domain.Entities.Product;

namespace Shopping.Application.Features.Product.Queries;

public record GetProductDetailByIdQuery(Guid Id)
    :IRequest<OperationResult<GetProductDetailByIdQueryResult>>,IValidatableModel<GetProductDetailByIdQuery>
{
    public IValidator<GetProductDetailByIdQuery> Validator(ValidationModelBase<GetProductDetailByIdQuery> validator)
    {
        validator.RuleFor(p => p.Id).NotEmpty();
        
        return validator;
    }
}