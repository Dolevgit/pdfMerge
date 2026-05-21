namespace PdfMerge.Infrastructure.Files;

public sealed class PortableAppPaths
{
    private PortableAppPaths(string applicationDirectory)
    {
        ApplicationDirectory = applicationDirectory;
        DataDirectory = Path.Combine(applicationDirectory, "data");
        LogsDirectory = Path.Combine(DataDirectory, "logs");
        SettingsFilePath = Path.Combine(DataDirectory, "settings.json");
    }

    public string ApplicationDirectory { get; }

    public string DataDirectory { get; }

    public string LogsDirectory { get; }

    public string SettingsFilePath { get; }

    public static PortableAppPaths FromBaseDirectory(string baseDirectory) =>
        new(Path.GetFullPath(baseDirectory));

    public void EnsureCreated()
    {
        Directory.CreateDirectory(DataDirectory);
        Directory.CreateDirectory(LogsDirectory);
    }
}
