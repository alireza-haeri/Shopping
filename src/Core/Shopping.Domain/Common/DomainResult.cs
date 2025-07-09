namespace Shopping.Domain.Common;

public record DomainResult(bool IsSuccess, string? ErrorMessage)
{
    public static readonly DomainResult Success = new DomainResult(false, null);
    public static DomainResult Failed(string? errorMessage = null) => new DomainResult(false, errorMessage);
}