using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PdfMerge.Application.Localization;
using PdfMerge.Application.Merging;
using PdfMerge.Application.Settings;
using PdfMerge.Application.Validation;
using PdfMerge.App.Services;
using PdfMerge.App.ViewModels;
using PdfMerge.App.Views;
using PdfMerge.Infrastructure.Files;
using PdfMerge.Infrastructure.Logging;
using PdfMerge.Infrastructure.Pdf;
using PdfMerge.Infrastructure.Settings;

namespace PdfMerge.App;

public partial class App : System.Windows.Application
{
    private ServiceProvider? _serviceProvider;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var paths = PortableAppPaths.FromBaseDirectory(AppContext.BaseDirectory);
        paths.EnsureCreated();

        var services = new ServiceCollection();
        ConfigureServices(services, paths);

        _serviceProvider = services.BuildServiceProvider();

        var logger = _serviceProvider.GetRequiredService<ILogger<App>>();
        logger.LogInformation("Application starting.");

        var viewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();
        await viewModel.InitializeAsync(CancellationToken.None).ConfigureAwait(true);

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        var logger = _serviceProvider?.GetService<ILogger<App>>();
        logger?.LogInformation("Application exiting.");

        _serviceProvider?.Dispose();
        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services, PortableAppPaths paths)
    {
        services.AddSingleton(paths);
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddProvider(new PortableFileLoggerProvider(paths.LogsDirectory));
        });

        services.AddSingleton<IApplicationLifetime, WpfApplicationLifetime>();
        services.AddSingleton<IFileDialogService, WpfFileDialogService>();
        services.AddSingleton<ILocalizationService, JsonLocalizationService>();
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<ISettingsService, JsonSettingsService>();
        services.AddSingleton<IPdfInputValidator, PdfInputValidator>();
        services.AddSingleton<IPdfMergeService, PdfSharpMergeService>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>();
    }
}
