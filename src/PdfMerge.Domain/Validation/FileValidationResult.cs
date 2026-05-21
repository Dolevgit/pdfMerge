namespace PdfMerge.Domain.Validation;

public sealed record FileValidationResult(bool IsValid, string? MessageKey)
{
    public static FileValidationResult Valid { get; } = new(true, null);

    public static FileValidationResult Invalid(string messageKey) => new(false, messageKey);
}
