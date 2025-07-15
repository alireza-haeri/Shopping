using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

namespace Shopping.Application.Extensions;

public static class ApplicationValidationExtensions
{
    public static List<KeyValuePair<string, string>> ConvertKeyValuePairs([NotNull]this List<ValidationFailure> failures)
    {
        return failures.Select(failure => new KeyValuePair<string, string>(failure.PropertyName, failure.ErrorMessage)).ToList();
    }
}