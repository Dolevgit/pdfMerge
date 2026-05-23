namespace PdfMerge.Application.Merging;

public sealed class PdfMergeException : Exception
{
    public PdfMergeException(string messageKey, Exception? innerException = null)
        : base(messageKey, innerException)
    {
        MessageKey = messageKey;
    }

    public string MessageKey { get; }
}
