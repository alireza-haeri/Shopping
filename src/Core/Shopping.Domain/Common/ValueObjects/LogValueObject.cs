namespace Shopping.Domain.Common.ValueObjects;

public record LogValueObject(DateTime EntryDate, string Message, string? AdditionalDescription = null)
{
    public static LogValueObject Log(string message, string? additionalDescription = null) =>
        new LogValueObject(DateTime.Now, message, additionalDescription);
}