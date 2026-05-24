using System.Windows;
using Microsoft.Win32;
using PdfMerge.Domain.Settings;

namespace PdfMerge.App.Services;

public sealed class WpfThemeService : IThemeService
{
    private const string LightThemePath = "/Assets/Theme.Light.xaml";
    private const string DarkThemePath = "/Assets/Theme.Dark.xaml";
    private const string PersonalizeRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string AppsUseLightThemeValue = "AppsUseLightTheme";

    public void ApplyTheme(string themePreference)
    {
        var resolvedTheme = ResolveTheme(themePreference);
        var source = resolvedTheme == ThemePreferences.Dark ? DarkThemePath : LightThemePath;

        var resources = System.Windows.Application.Current.Resources.MergedDictionaries;
        for (var index = resources.Count - 1; index >= 0; index--)
        {
            var existingSource = resources[index].Source?.OriginalString;
            if (string.Equals(existingSource, LightThemePath, StringComparison.OrdinalIgnoreCase)
                || string.Equals(existingSource, DarkThemePath, StringComparison.OrdinalIgnoreCase))
            {
                resources.RemoveAt(index);
            }
        }

        resources.Insert(0, new ResourceDictionary
        {
            Source = new Uri(source, UriKind.Relative)
        });
    }

    private static string ResolveTheme(string themePreference)
    {
        if (string.Equals(themePreference, ThemePreferences.Light, StringComparison.OrdinalIgnoreCase))
        {
            return ThemePreferences.Light;
        }

        if (string.Equals(themePreference, ThemePreferences.Dark, StringComparison.OrdinalIgnoreCase))
        {
            return ThemePreferences.Dark;
        }

        return IsSystemDarkTheme() ? ThemePreferences.Dark : ThemePreferences.Light;
    }

    private static bool IsSystemDarkTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(PersonalizeRegistryPath);
            return key?.GetValue(AppsUseLightThemeValue) is int appsUseLightTheme && appsUseLightTheme == 0;
        }
        catch (Exception exception) when (exception is UnauthorizedAccessException or System.Security.SecurityException or System.IO.IOException)
        {
            return false;
        }
    }
}
