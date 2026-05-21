namespace PdfMerge.Domain.Settings;

public static class WindowStateNames
{
    public const string Normal = "normal";
    public const string Maximized = "maximized";

    public static bool IsSupported(string? windowState) =>
        string.Equals(windowState, Normal, StringComparison.OrdinalIgnoreCase)
        || string.Equals(windowState, Maximized, StringComparison.OrdinalIgnoreCase);
}
