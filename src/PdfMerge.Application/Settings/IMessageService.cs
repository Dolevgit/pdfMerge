namespace PdfMerge.Application.Settings;

public interface IMessageService
{
    event EventHandler<UserMessageChangedEventArgs>? MessageChanged;

    void ShowStatus(string message);

    void ShowSuccess(string message);

    void ShowError(string message);

    void Dismiss();
}
