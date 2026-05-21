using Microsoft.Extensions.Logging;

namespace PdfMerge.Infrastructure.Logging;

public sealed class PortableFileLoggerProvider : ILoggerProvider
{
    private readonly object _syncRoot = new();
    private readonly string _logsDirectory;

    public PortableFileLoggerProvider(string logsDirectory)
    {
        _logsDirectory = logsDirectory;
        Directory.CreateDirectory(_logsDirectory);
    }

    public ILogger CreateLogger(string categoryName) => new PortableFileLogger(categoryName, _logsDirectory, _syncRoot);

    public void Dispose()
    {
    }

    private sealed class PortableFileLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _logsDirectory;
        private readonly object _syncRoot;

        public PortableFileLogger(string categoryName, string logsDirectory, object syncRoot)
        {
            _categoryName = categoryName;
            _logsDirectory = logsDirectory;
            _syncRoot = syncRoot;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull =>
            NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var line = FormatLine(logLevel, eventId, formatter(state, exception), exception);
            var path = Path.Combine(_logsDirectory, $"pdfmerge-{DateTimeOffset.Now:yyyyMMdd}.log");

            lock (_syncRoot)
            {
                File.AppendAllText(path, line);
            }
        }

        private string FormatLine(LogLevel logLevel, EventId eventId, string message, Exception? exception)
        {
            var eventPart = eventId.Id == 0 ? string.Empty : $" eventId={eventId.Id}";
            var exceptionPart = exception is null
                ? string.Empty
                : $" exception=\"{exception.GetType().FullName}: {exception.Message}\"";

            return $"{DateTimeOffset.Now:O} level={logLevel} category=\"{_categoryName}\"{eventPart} message=\"{Escape(message)}\"{exceptionPart}{Environment.NewLine}";
        }

        private static string Escape(string value) => value.Replace("\"", "\\\"", StringComparison.Ordinal);
    }

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();

        public void Dispose()
        {
        }
    }
}
