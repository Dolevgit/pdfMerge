namespace PdfMerge.Application.Settings;

public sealed class UserMessageChangedEventArgs : EventArgs
{
    public UserMessageChangedEventArgs(string statusText, string? messageText, UserMessageKind messageKind)
    {
        StatusText = statusText;
        MessageText = messageText;
        MessageKind = messageKind;
    }

    public string StatusText { get; }

    public string? MessageText { get; }

    public UserMessageKind MessageKind { get; }
}
