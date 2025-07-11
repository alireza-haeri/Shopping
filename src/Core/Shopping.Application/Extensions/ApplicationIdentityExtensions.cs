using Microsoft.AspNetCore.Identity;
using Shopping.Domain.Entities.Product;

namespace Shopping.Application.Extensions;

public static class ApplicationIdentityExtensions
{
    public static List<KeyValuePair<string, string>> ConvertToKeyValuePairs(this IEnumerable<IdentityError> errors)
    {
        return errors.Select(error => new KeyValuePair<string, string>("GeneralErrors", error.Description)).ToList();
    }
}