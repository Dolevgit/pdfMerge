using System.ComponentModel;
using System.Windows;
using PdfMerge.App.ViewModels;
using PdfMerge.Domain.Settings;

namespace PdfMerge.App.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    private bool _isClosingAfterSave;

    public MainWindow(MainWindowViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = _viewModel;
        InitializeComponent();
        Loaded += OnLoaded;
        Closing += OnClosing;
        FileNameColumn.Header = _viewModel.FileNameHeaderText;
        FullPathColumn.Header = _viewModel.FullPathHeaderText;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var settings = _viewModel.Settings;
        Width = settings.LastNormalWindowWidth;
        Height = settings.LastNormalWindowHeight;
        Left = settings.LastNormalWindowLeft;
        Top = settings.LastNormalWindowTop;

        if (string.Equals(settings.WindowState, WindowStateNames.Maximized, StringComparison.OrdinalIgnoreCase))
        {
            WindowState = WindowState.Maximized;
        }
    }

    private async void OnClosing(object? sender, CancelEventArgs e)
    {
        if (_isClosingAfterSave)
        {
            return;
        }

        e.Cancel = true;
        CaptureWindowPlacement();
        await _viewModel.SaveSettingsAsync(CancellationToken.None).ConfigureAwait(true);
        _isClosingAfterSave = true;
        Close();
    }

    private void CaptureWindowPlacement()
    {
        var state = WindowState == WindowState.Maximized ? WindowStateNames.Maximized : WindowStateNames.Normal;
        var bounds = WindowState == WindowState.Normal ? new Rect(Left, Top, Width, Height) : RestoreBounds;

        _viewModel.UpdateWindowPlacement(state, bounds.Width, bounds.Height, bounds.Left, bounds.Top);
    }
}
