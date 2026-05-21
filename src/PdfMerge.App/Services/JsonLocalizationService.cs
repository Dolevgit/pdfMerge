using System.Globalization;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PdfMerge.Application.Localization;
using PdfMerge.Domain.Settings;

namespace PdfMerge.App.Services;

public sealed class JsonLocalizationService : ILocalizationService
{
    private readonly ILogger<JsonLocalizationService> _logger;
    private readonly string _localesDirectory;
    private readonly Dictionary<string, string> _englishStrings;
    private Dictionary<string, string> _currentStrings;

    public JsonLocalizationService(ILogger<JsonLocalizationService> logger)
    {
        _logger = logger;
        _localesDirectory = Path.Combine(AppContext.BaseDirectory, "Locales");
        _englishStrings = LoadLanguage(SupportedLanguages.English);
        _currentStrings = _englishStrings;
        CurrentLanguage = SupportedLanguages.English;
    }

    public string CurrentLanguage { get; private set; }

    public CultureDirection Direction =>
        string.Equals(CurrentLanguage, SupportedLanguages.Hebrew, StringComparison.OrdinalIgnoreCase)
            ? CultureDirection.RightToLeft
            : CultureDirection.LeftToRight;

    public void SetLanguage(string language)
    {
        var requestedLanguage = SupportedLanguages.IsSupported(language) ? language : SupportedLanguages.English;
        var strings = LoadLanguage(requestedLanguage);

        CurrentLanguage = requestedLanguage;
        _currentStrings = strings.Count == 0 ? _englishStrings : strings;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(CurrentLanguage);
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(CurrentLanguage);
    }

    public string GetString(string key)
    {
        if (_currentStrings.TryGetValue(key, out var value))
        {
            return value;
        }

        return _englishStrings.TryGetValue(key, out var fallbackValue) ? fallbackValue : key;
    }

    private Dictionary<string, string> LoadLanguage(string language)
    {
        var path = Path.Combine(_localesDirectory, $"{language}.json");

        try
        {
            if (!File.Exists(path))
            {
                _logger.LogWarning("Localization file is missing for language {Language}.", language);
                return new Dictionary<string, string>(StringComparer.Ordinal);
            }

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                ?? new Dictionary<string, string>(StringComparer.Ordinal);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
        {
            _logger.LogError(exception, "Failed to load localization for language {Language}.", language);
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }
    }
}
