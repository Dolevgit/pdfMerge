namespace PdfMerge.Application.Localization;

public interface ILocalizationService
{
    string CurrentLanguage { get; }

    CultureDirection Direction { get; }

    void SetLanguage(string language);

    string GetString(string key);
}
