using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shopping.Application.Common.Validation;

namespace Shopping.Application.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationValidator(this IServiceCollection services)
    {
        var validationTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetExportedTypes())
            .Where(a => a.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidatableModel<>)))
            .ToList();

        foreach (var validationType in validationTypes)
        {
            var biggestConstructorLength =
                validationType.GetConstructors().OrderByDescending(c => c.GetParameters().Length)
                    .First().GetParameters().Length;

            var requestModel = Activator.CreateInstance(validationType, new object[biggestConstructorLength]);
            if (requestModel is null) continue;

            var requestMethodInfo = validationType.GetMethod(nameof(IValidatableModel<object>.Validator));
            var validationModelBase =
                Activator.CreateInstance(typeof(ValidationModelBase<>).MakeGenericType(validationType));
            if (validationModelBase is null) continue;

            var validator = requestMethodInfo?.Invoke(requestModel, [validationModelBase]);
            if (validator is null) continue;

            var validatorInterfaces = validator
                .GetType()
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
            if (validatorInterfaces is null) continue;

            services.AddTransient(validatorInterfaces, _ => validator);
        }

        return services;
    }
}