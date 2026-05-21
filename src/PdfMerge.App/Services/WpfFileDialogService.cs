using System.IO;
using Microsoft.Win32;
using PdfMerge.Application.Localization;
using PdfMerge.Application.Settings;

namespace PdfMerge.App.Services;

public sealed class WpfFileDialogService : IFileDialogService
{
    private readonly ILocalizationService _localizationService;

    public WpfFileDialogService(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }

    public IReadOnlyList<string> ShowOpenPdfDialog(string? initialDirectory)
    {
        var dialog = new OpenFileDialog
        {
            Filter = GetPdfFilter(),
            Multiselect = true,
            InitialDirectory = Directory.Exists(initialDirectory) ? initialDirectory : null
        };

        return dialog.ShowDialog() == true ? dialog.FileNames : Array.Empty<string>();
    }

    public string? ShowSavePdfDialog(string? initialDirectory)
    {
        var dialog = new SaveFileDialog
        {
            Filter = GetPdfFilter(),
            AddExtension = true,
            DefaultExt = ".pdf",
            InitialDirectory = Directory.Exists(initialDirectory) ? initialDirectory : null
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    private string GetPdfFilter() => $"{_localizationService.GetString("dialog.filterPdf")}|*.pdf";
}
