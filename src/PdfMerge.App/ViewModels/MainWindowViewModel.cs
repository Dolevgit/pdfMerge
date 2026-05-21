using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using PdfMerge.Application.Localization;
using PdfMerge.Application.Settings;
using PdfMerge.App.Services;
using PdfMerge.Domain.Settings;

namespace PdfMerge.App.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly ISettingsService _settingsService;
    private readonly ILocalizationService _localizationService;
    private readonly IMessageService _messageService;
    private readonly IApplicationLifetime _applicationLifetime;
    private readonly ILogger<MainWindowViewModel> _logger;
    private string _statusText = string.Empty;
    private string? _messageText;
    private UserMessageKind _messageKind;
    private FlowDirection _flowDirection = FlowDirection.LeftToRight;
    private AppSettings _settings = AppSettings.CreateDefault();

    public MainWindowViewModel(
        ISettingsService settingsService,
        ILocalizationService localizationService,
        IMessageService messageService,
        IApplicationLifetime applicationLifetime,
        ILogger<MainWindowViewModel> logger)
    {
        _settingsService = settingsService;
        _localizationService = localizationService;
        _messageService = messageService;
        _applicationLifetime = applicationLifetime;
        _logger = logger;

        SelectedFiles = new ObservableCollection<SelectedPdfViewModel>();
        AddPdfsCommand = new RelayCommand(ShowDeferredFeatureMessage);
        SettingsCommand = new RelayCommand(ShowDeferredFeatureMessage);
        ExitCommand = new RelayCommand(_applicationLifetime.Shutdown);
        MoveUpCommand = new RelayCommand(ShowDeferredFeatureMessage, () => false);
        MoveDownCommand = new RelayCommand(ShowDeferredFeatureMessage, () => false);
        RemoveCommand = new RelayCommand(ShowDeferredFeatureMessage, () => false);
        SelectAllCommand = new RelayCommand(ShowDeferredFeatureMessage, () => false);
        MergeCommand = new RelayCommand(ShowDeferredFeatureMessage, () => false);
        OpenGitHubProjectCommand = new RelayCommand(ShowDeferredFeatureMessage, () => false);
        AboutCommand = new RelayCommand(() => _messageService.ShowSuccess(T("message.about")));
        DismissMessageCommand = new RelayCommand(_messageService.Dismiss);

        _messageService.MessageChanged += OnMessageChanged;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<SelectedPdfViewModel> SelectedFiles { get; }

    public ICommand AddPdfsCommand { get; }

    public ICommand SettingsCommand { get; }

    public ICommand ExitCommand { get; }

    public ICommand MoveUpCommand { get; }

    public ICommand MoveDownCommand { get; }

    public ICommand RemoveCommand { get; }

    public ICommand SelectAllCommand { get; }

    public ICommand MergeCommand { get; }

    public ICommand OpenGitHubProjectCommand { get; }

    public ICommand AboutCommand { get; }

    public ICommand DismissMessageCommand { get; }

    public AppSettings Settings => _settings;

    public string Title => T("app.title");

    public string FileMenuText => T("menu.file");

    public string EditMenuText => T("menu.edit");

    public string HelpMenuText => T("menu.help");

    public string AddPdfsText => T("command.addPdfs");

    public string SettingsText => T("command.settings");

    public string ExitText => T("command.exit");

    public string MoveUpText => T("command.moveUp");

    public string MoveDownText => T("command.moveDown");

    public string RemoveText => T("command.remove");

    public string SelectAllText => T("command.selectAll");

    public string MergeText => T("command.merge");

    public string OpenGitHubProjectText => T("command.openGitHubProject");

    public string AboutText => T("command.about");

    public string SelectedFilesText => T("label.selectedFiles");

    public string FileNameHeaderText => T("column.fileName");

    public string FullPathHeaderText => T("column.fullPath");

    public string EmptyListText => T("message.emptyList");

    public string DismissText => T("command.dismiss");

    public string StatusText
    {
        get => _statusText;
        private set => SetField(ref _statusText, value);
    }

    public string? MessageText
    {
        get => _messageText;
        private set
        {
            if (SetField(ref _messageText, value))
            {
                OnPropertyChanged(nameof(HasMessage));
            }
        }
    }

    public bool HasMessage => !string.IsNullOrWhiteSpace(MessageText);

    public UserMessageKind MessageKind
    {
        get => _messageKind;
        private set => SetField(ref _messageKind, value);
    }

    public FlowDirection FlowDirection
    {
        get => _flowDirection;
        private set => SetField(ref _flowDirection, value);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        _settings = await _settingsService.LoadAsync(cancellationToken).ConfigureAwait(true);
        _localizationService.SetLanguage(_settings.Language);
        FlowDirection = _localizationService.Direction == CultureDirection.RightToLeft
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;

        RaiseLocalizedPropertiesChanged();
        _messageService.ShowStatus(T("status.ready"));
    }

    public async Task SaveSettingsAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _settingsService.SaveAsync(_settings, cancellationToken).ConfigureAwait(true);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            _logger.LogError(exception, "Settings could not be saved during shutdown.");
        }
    }

    public void UpdateWindowPlacement(string windowState, double width, double height, double left, double top)
    {
        _settings.WindowState = windowState;
        _settings.LastNormalWindowWidth = width;
        _settings.LastNormalWindowHeight = height;
        _settings.LastNormalWindowLeft = left;
        _settings.LastNormalWindowTop = top;
    }

    private void ShowDeferredFeatureMessage() => _messageService.ShowError(T("message.featureNotAvailable"));

    private void OnMessageChanged(object? sender, UserMessageChangedEventArgs e)
    {
        StatusText = e.StatusText;
        MessageText = e.MessageText;
        MessageKind = e.MessageKind;
    }

    private string T(string key) => _localizationService.GetString(key);

    private void RaiseLocalizedPropertiesChanged()
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(FileMenuText));
        OnPropertyChanged(nameof(EditMenuText));
        OnPropertyChanged(nameof(HelpMenuText));
        OnPropertyChanged(nameof(AddPdfsText));
        OnPropertyChanged(nameof(SettingsText));
        OnPropertyChanged(nameof(ExitText));
        OnPropertyChanged(nameof(MoveUpText));
        OnPropertyChanged(nameof(MoveDownText));
        OnPropertyChanged(nameof(RemoveText));
        OnPropertyChanged(nameof(SelectAllText));
        OnPropertyChanged(nameof(MergeText));
        OnPropertyChanged(nameof(OpenGitHubProjectText));
        OnPropertyChanged(nameof(AboutText));
        OnPropertyChanged(nameof(SelectedFilesText));
        OnPropertyChanged(nameof(FileNameHeaderText));
        OnPropertyChanged(nameof(FullPathHeaderText));
        OnPropertyChanged(nameof(EmptyListText));
        OnPropertyChanged(nameof(DismissText));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
