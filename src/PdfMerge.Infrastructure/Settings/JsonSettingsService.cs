using System.Text.Json;
using Microsoft.Extensions.Logging;
using PdfMerge.Application.Settings;
using PdfMerge.Domain.Settings;
using PdfMerge.Infrastructure.Files;

namespace PdfMerge.Infrastructure.Settings;

public sealed class JsonSettingsService : ISettingsService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private readonly PortableAppPaths _paths;
    private readonly ILogger<JsonSettingsService> _logger;

    public JsonSettingsService(PortableAppPaths paths, ILogger<JsonSettingsService> logger)
    {
        _paths = paths;
        _logger = logger;
    }

    public async Task<AppSettings> LoadAsync(CancellationToken cancellationToken)
    {
        _paths.EnsureCreated();

        if (!File.Exists(_paths.SettingsFilePath))
        {
            var defaultSettings = AppSettings.CreateDefault();
            await SaveAsync(defaultSettings, cancellationToken).ConfigureAwait(false);
            return defaultSettings;
        }

        try
        {
            await using var stream = File.OpenRead(_paths.SettingsFilePath);
            var settings = await JsonSerializer.DeserializeAsync<AppSettings>(stream, SerializerOptions, cancellationToken)
                .ConfigureAwait(false);

            return Validate(settings ?? AppSettings.CreateDefault());
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
        {
            _logger.LogError(exception, "Failed to load settings; defaults will be used.");
            return AppSettings.CreateDefault();
        }
    }

    public async Task SaveAsync(AppSettings settings, CancellationToken cancellationToken)
    {
        _paths.EnsureCreated();
        var validatedSettings = Validate(settings);
        var temporaryPath = $"{_paths.SettingsFilePath}.tmp";

        try
        {
            await using (var stream = File.Create(temporaryPath))
            {
                await JsonSerializer.SerializeAsync(stream, validatedSettings, SerializerOptions, cancellationToken)
                    .ConfigureAwait(false);
            }

            File.Move(temporaryPath, _paths.SettingsFilePath, overwrite: true);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
        {
            _logger.LogError(exception, "Failed to save settings.");
            TryDeleteTemporaryFile(temporaryPath);
            throw;
        }
    }

    private static AppSettings Validate(AppSettings settings)
    {
        if (settings.SchemaVersion < 1)
        {
            settings.SchemaVersion = 1;
        }

        if (!SupportedLanguages.IsSupported(settings.Language))
        {
            settings.Language = SupportedLanguages.English;
        }

        if (!ThemePreferences.IsSupported(settings.Theme))
        {
            settings.Theme = ThemePreferences.System;
        }

        if (!WindowStateNames.IsSupported(settings.WindowState))
        {
            settings.WindowState = WindowStateNames.Normal;
        }

        settings.LastNormalWindowWidth = Clamp(settings.LastNormalWindowWidth, 800, 3840);
        settings.LastNormalWindowHeight = Clamp(settings.LastNormalWindowHeight, 520, 2160);

        if (double.IsNaN(settings.LastNormalWindowLeft) || double.IsInfinity(settings.LastNormalWindowLeft))
        {
            settings.LastNormalWindowLeft = 100;
        }

        if (double.IsNaN(settings.LastNormalWindowTop) || double.IsInfinity(settings.LastNormalWindowTop))
        {
            settings.LastNormalWindowTop = 100;
        }

        return settings;
    }

    private static double Clamp(double value, double minimum, double maximum)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return minimum;
        }

        return Math.Min(Math.Max(value, minimum), maximum);
    }

    private static void TryDeleteTemporaryFile(string temporaryPath)
    {
        try
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
