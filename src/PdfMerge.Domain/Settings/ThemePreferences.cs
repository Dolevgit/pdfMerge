namespace PdfMerge.Domain.Settings;

public static class ThemePreferences
{
    public const string System = "system";
    public const string Light = "light";
    public const string Dark = "dark";

    public static bool IsSupported(string? theme) =>
        string.Equals(theme, System, StringComparison.OrdinalIgnoreCase)
        || string.Equals(theme, Light, StringComparison.OrdinalIgnoreCase)
        || string.Equals(theme, Dark, StringComparison.OrdinalIgnoreCase);
}
