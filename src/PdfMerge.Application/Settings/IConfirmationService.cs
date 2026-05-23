namespace PdfMerge.Application.Settings;

public interface IConfirmationService
{
    bool ConfirmOverwrite(string filePath);
}
