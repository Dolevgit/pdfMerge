namespace PdfMerge.Application.Settings;

public sealed class UserMessageChangedEventArgs : EventArgs
{
    public UserMessageChangedEventArgs(string statusText, UserMessageKind messageKind)
    {
        StatusText = statusText;
        MessageKind = messageKind;
    }

    public string StatusText { get; }

    public UserMessageKind MessageKind { get; }
}
