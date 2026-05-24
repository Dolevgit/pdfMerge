using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using PdfMerge.App.ViewModels;
using PdfMerge.Domain.Settings;

namespace PdfMerge.App.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    private bool _isApplyingSavedPlacement;
    private bool _isClosingAfterSave;

    public MainWindow(MainWindowViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = _viewModel;
        InitializeComponent();
        Loaded += OnLoaded;
        Closing += OnClosing;
        StateChanged += OnStateChanged;
        LocationChanged += OnLocationChanged;
        SizeChanged += OnSizeChanged;
        _viewModel.SelectAllRequested += OnSelectAllRequested;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _isApplyingSavedPlacement = true;
        var settings = _viewModel.Settings;
        Width = settings.LastNormalWindowWidth;
        Height = settings.LastNormalWindowHeight;
        Left = settings.LastNormalWindowLeft;
        Top = settings.LastNormalWindowTop;

        if (string.Equals(settings.WindowState, WindowStateNames.Maximized, StringComparison.OrdinalIgnoreCase))
        {
            WindowState = WindowState.Maximized;
        }

        _isApplyingSavedPlacement = false;
        CaptureWindowPlacement();
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
        _ = Dispatcher.BeginInvoke(Close, DispatcherPriority.Background);
    }

    private void CaptureWindowPlacement()
    {
        var state = WindowState == WindowState.Maximized ? WindowStateNames.Maximized : WindowStateNames.Normal;
        var bounds = WindowState == WindowState.Normal ? new Rect(Left, Top, Width, Height) : RestoreBounds;

        _viewModel.UpdateWindowPlacement(state, bounds.Width, bounds.Height, bounds.Left, bounds.Top);
    }

    private async void OnStateChanged(object? sender, EventArgs e)
    {
        if (_isApplyingSavedPlacement || WindowState == WindowState.Minimized)
        {
            return;
        }

        CaptureWindowPlacement();
        await _viewModel.SaveSettingsAsync(CancellationToken.None).ConfigureAwait(true);
    }

    private void OnLocationChanged(object? sender, EventArgs e)
    {
        CaptureNormalWindowPlacement();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        CaptureNormalWindowPlacement();
    }

    private void CaptureNormalWindowPlacement()
    {
        if (_isApplyingSavedPlacement || WindowState != WindowState.Normal)
        {
            return;
        }

        _viewModel.UpdateWindowPlacement(WindowStateNames.Normal, Width, Height, Left, Top);
    }

    private void OnSelectedFilesSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _viewModel.SetSelectedFiles(SelectedFilesListView.SelectedItems.Cast<SelectedPdfViewModel>());
    }

    private void OnSelectAllRequested(object? sender, EventArgs e)
    {
        SelectedFilesListView.SelectAll();
    }

    private void OnPreviewDragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private async void OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
        {
            await _viewModel.AddPdfPathsAsync(paths, CancellationToken.None).ConfigureAwait(true);
        }

        e.Handled = true;
    }
}
