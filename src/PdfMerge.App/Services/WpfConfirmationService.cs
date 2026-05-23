using System.Windows;
using PdfMerge.Application.Localization;
using PdfMerge.Application.Settings;

namespace PdfMerge.App.Services;

public sealed class WpfConfirmationService : IConfirmationService
{
    private readonly ILocalizationService _localizationService;

    public WpfConfirmationService(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public bool ConfirmOverwrite(string filePath)
    {
        var message = string.Format(
            System.Globalization.CultureInfo.CurrentCulture,
            _localizationService.GetString("confirm.overwrite.message"),
            filePath);

        var result = MessageBox.Show(
            message,
            _localizationService.GetString("confirm.overwrite.title"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        return result == MessageBoxResult.Yes;
    }
}
