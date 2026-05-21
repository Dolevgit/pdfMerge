namespace PdfMerge.App.ViewModels;

public sealed class SelectedPdfViewModel
{
    public SelectedPdfViewModel(string fileName, string fullPath)
    {
        FileName = fileName;
        FullPath = fullPath;
    }

    public string FileName { get; }

    public string FullPath { get; }
}
