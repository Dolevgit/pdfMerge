using System.Windows.Threading;
using PdfMerge.Application.Settings;

namespace PdfMerge.App.Services;

public sealed class MessageService : IMessageService
{
    private readonly DispatcherTimer _dismissTimer;
    private string _statusText = string.Empty;

    public MessageService()
    {
        _dismissTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _dismissTimer.Tick += (_, _) => Dismiss();
    }

    public event EventHandler<UserMessageChangedEventArgs>? MessageChanged;

    public void ShowStatus(string message)
    {
        _statusText = message;
        _dismissTimer.Stop();
        MessageChanged?.Invoke(this, new UserMessageChangedEventArgs(_statusText, null, UserMessageKind.None));
    }

    public void ShowSuccess(string message) => ShowDismissible(message, UserMessageKind.Success);

    public void ShowError(string message) => ShowDismissible(message, UserMessageKind.Error);

    public void Dismiss()
    {
        _dismissTimer.Stop();
        MessageChanged?.Invoke(this, new UserMessageChangedEventArgs(_statusText, null, UserMessageKind.None));
    }

    private void ShowDismissible(string message, UserMessageKind kind)
    {
        _statusText = message;
        _dismissTimer.Stop();
        MessageChanged?.Invoke(this, new UserMessageChangedEventArgs(_statusText, message, kind));
        _dismissTimer.Start();
    }
}
