using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using PdfMerge.App.ViewModels;

namespace PdfMerge.App.Views;

public partial class AboutWindow : Window
{
    private const string AuthorGitHubUrl = "https://github.com/Dolevgit";
    private const string ProjectGitHubUrl = "https://github.com/Dolevgit/pdfMerge";

    private readonly MainWindowViewModel _viewModel;

    public AboutWindow(MainWindowViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();

        FlowDirection = _viewModel.FlowDirection;
        Title = _viewModel.AboutWindowTitleText;
        AppNameTextBlock.Text = _viewModel.Title;
        VersionInfoTextBlock.Text = string.Format(CultureInfo.CurrentCulture, _viewModel.AboutVersionText, ResolveVersionText());
        LicenseInfoTextBlock.Text = _viewModel.AboutLicenseText;
        ProjectGitHubLink.Inlines.Clear();
        ProjectGitHubLink.Inlines.Add(_viewModel.AboutOpenGitHubProjectText);
        BuiltWithCodexTextBlock.Text = _viewModel.AboutBuiltWithCodexText;
        OkButton.Content = _viewModel.AboutOkText;
    }

    private void AuthorGitHubLink_OnClick(object sender, RoutedEventArgs e)
    {
        OpenUrl(AuthorGitHubUrl);
    }

    private void ProjectGitHubLink_OnClick(object sender, RoutedEventArgs e)
    {
        OpenUrl(ProjectGitHubUrl);
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OpenUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                this,
                string.Format(CultureInfo.CurrentCulture, _viewModel.AboutOpenLinkFailedMessage, exception.Message),
                _viewModel.AboutOpenLinkFailedTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private static string ResolveVersionText()
    {
        var assembly = typeof(AboutWindow).Assembly;
        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            var metadataSeparatorIndex = informationalVersion.IndexOf('+', StringComparison.Ordinal);
            return metadataSeparatorIndex >= 0
                ? informationalVersion[..metadataSeparatorIndex]
                : informationalVersion;
        }

        var version = assembly.GetName().Version;
        return version is null
            ? "Unknown"
            : version.ToString();
    }
}
