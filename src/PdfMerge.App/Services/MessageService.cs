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
        _dismissTimer.Tick += (_, _) => ResetMessageKind();
    }

    public event EventHandler<UserMessageChangedEventArgs>? MessageChanged;

    public void ShowStatus(string message)
    {
        _statusText = message;
        _dismissTimer.Stop();
        MessageChanged?.Invoke(this, new UserMessageChangedEventArgs(_statusText, UserMessageKind.None));
    }

    public void ShowSuccess(string message) => ShowTransientStatus(message, UserMessageKind.Success);

    public void ShowError(string message) => ShowTransientStatus(message, UserMessageKind.Error);

    private void ResetMessageKind()
    {
        _dismissTimer.Stop();
        MessageChanged?.Invoke(this, new UserMessageChangedEventArgs(_statusText, UserMessageKind.None));
    }

    private void ShowTransientStatus(string message, UserMessageKind kind)
    {
        _statusText = message;
        _dismissTimer.Stop();
        MessageChanged?.Invoke(this, new UserMessageChangedEventArgs(_statusText, kind));
        _dismissTimer.Start();
    }
}
