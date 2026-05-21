using System.Text.Json;
using System.Text.Json.Serialization;

namespace PdfMerge.Domain.Settings;

public sealed class AppSettings
{
    public int SchemaVersion { get; set; } = 1;

    public string Language { get; set; } = SupportedLanguages.English;

    public string Theme { get; set; } = ThemePreferences.System;

    public string WindowState { get; set; } = WindowStateNames.Normal;

    public double LastNormalWindowWidth { get; set; } = 1000;

    public double LastNormalWindowHeight { get; set; } = 680;

    public double LastNormalWindowLeft { get; set; } = 100;

    public double LastNormalWindowTop { get; set; } = 100;

    public string? LastInputFolder { get; set; }

    public string? LastOutputFolder { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }

    public static AppSettings CreateDefault() => new();
}
