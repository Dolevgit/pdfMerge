using System.Windows;

namespace PdfMerge.App.Services;

public sealed class WpfApplicationLifetime : IApplicationLifetime
{
    public void Shutdown() => System.Windows.Application.Current.Shutdown();
}
