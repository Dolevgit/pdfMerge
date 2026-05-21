namespace PdfMerge.Domain.Settings;

public static class SupportedLanguages
{
    public const string English = "en";
    public const string Hebrew = "he";

    public static bool IsSupported(string? language) =>
        string.Equals(language, English, StringComparison.OrdinalIgnoreCase)
        || string.Equals(language, Hebrew, StringComparison.OrdinalIgnoreCase);
}
