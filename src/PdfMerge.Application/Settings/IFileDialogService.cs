namespace PdfMerge.Application.Settings;

public interface IFileDialogService
{
    IReadOnlyList<string> ShowOpenPdfDialog(string? initialDirectory);

    string? ShowSavePdfDialog(string? initialDirectory);
}
