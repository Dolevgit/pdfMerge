using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using PdfMerge.Application.Localization;
using PdfMerge.Application.Settings;
using PdfMerge.Application.Validation;
using PdfMerge.App.Services;
using PdfMerge.Domain.Files;
using PdfMerge.Domain.Settings;

namespace PdfMerge.App.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly ISettingsService _settingsService;
    private readonly ILocalizationService _localizationService;
    private readonly IMessageService _messageService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IPdfInputValidator _pdfInputValidator;
    private readonly IApplicationLifetime _applicationLifetime;
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly List<SelectedPdfViewModel> _selectedFileItems = [];
    private string _statusText = string.Empty;
    private string? _messageText;
    private UserMessageKind _messageKind;
    private FlowDirection _flowDirection = FlowDirection.LeftToRight;
    private AppSettings _settings = AppSettings.CreateDefault();
    private bool _isAddingFiles;

    public MainWindowViewModel(
        ISettingsService settingsService,
        ILocalizationService localizationService,
        IMessageService messageService,
        IFileDialogService fileDialogService,
        IPdfInputValidator pdfInputValidator,
        IApplicationLifetime applicationLifetime,
        ILogger<MainWindowViewModel> logger)
    {
        _settingsService = settingsService;
        _localizationService = localizationService;
        _messageService = messageService;
        _fileDialogService = fileDialogService;
        _pdfInputValidator = pdfInputValidator;
        _applicationLifetime = applicationLifetime;
        _logger = logger;

        SelectedFiles = new ObservableCollection<SelectedPdfViewModel>();
        AddPdfsCommand = new RelayCommand(() => _ = AddPdfsFromDialogAsync(), () => !_isAddingFiles);
        SettingsCommand = new RelayCommand(ShowDeferredFeatureMessage);
        ExitCommand = new RelayCommand(_applicationLifetime.Shutdown);
        MoveUpCommand = new RelayCommand(MoveSelectedFileUp, CanMoveSelectedFileUp);
        MoveDownCommand = new RelayCommand(MoveSelectedFileDown, CanMoveSelectedFileDown);
        RemoveCommand = new RelayCommand(RemoveSelectedFiles, HasSelectedFiles);
        SelectAllCommand = new RelayCommand(RequestSelectAll, () => SelectedFiles.Count > 0);
        MergeCommand = new RelayCommand(ShowDeferredFeatureMessage, () => false);
        OpenGitHubProjectCommand = new RelayCommand(ShowDeferredFeatureMessage, () => false);
        AboutCommand = new RelayCommand(() => _messageService.ShowSuccess(T("message.about")));
        DismissMessageCommand = new RelayCommand(_messageService.Dismiss);

        _messageService.MessageChanged += OnMessageChanged;
        SelectedFiles.CollectionChanged += (_, _) => RaiseCommandStatesChanged();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler? SelectAllRequested;

    public ObservableCollection<SelectedPdfViewModel> SelectedFiles { get; }

    public RelayCommand AddPdfsCommand { get; }

    public ICommand SettingsCommand { get; }

    public ICommand ExitCommand { get; }

    public RelayCommand MoveUpCommand { get; }

    public RelayCommand MoveDownCommand { get; }

    public RelayCommand RemoveCommand { get; }

    public RelayCommand SelectAllCommand { get; }

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

    public void SetSelectedFiles(IEnumerable<SelectedPdfViewModel> selectedFiles)
    {
        _selectedFileItems.Clear();
        _selectedFileItems.AddRange(selectedFiles);
        RaiseCommandStatesChanged();
    }

    public async Task AddPdfPathsAsync(IEnumerable<string> paths, CancellationToken cancellationToken)
    {
        var pathList = paths
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToList();

        if (pathList.Count == 0)
        {
            return;
        }

        var addedCount = 0;
        var rejectedCount = 0;
        var duplicateCount = 0;
        string? singleRejectionMessageKey = null;
        string? lastAddedDirectory = null;

        foreach (var path in pathList)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!TryCreatePdfInputFile(path, out var inputFile))
            {
                rejectedCount++;
                singleRejectionMessageKey = "message.invalidFilePath";
                continue;
            }

            if (ContainsSelectedPath(inputFile.FullPath))
            {
                duplicateCount++;
                continue;
            }

            var validationResult = await _pdfInputValidator.ValidateAsync(inputFile, cancellationToken).ConfigureAwait(true);
            if (!validationResult.IsValid)
            {
                rejectedCount++;
                singleRejectionMessageKey = validationResult.MessageKey;
                continue;
            }

            SelectedFiles.Add(new SelectedPdfViewModel(inputFile.FileName, inputFile.FullPath));
            addedCount++;
            lastAddedDirectory = Path.GetDirectoryName(inputFile.FullPath);
        }

        if (!string.IsNullOrWhiteSpace(lastAddedDirectory))
        {
            _settings.LastInputFolder = lastAddedDirectory;
            await SaveSettingsAsync(cancellationToken).ConfigureAwait(true);
        }

        _logger.LogInformation(
            "PDF file add completed. Added: {AddedCount}; rejected: {RejectedCount}; duplicates: {DuplicateCount}.",
            addedCount,
            rejectedCount,
            duplicateCount);

        ShowAddFilesResult(addedCount, rejectedCount, duplicateCount, singleRejectionMessageKey);
    }

    private void ShowDeferredFeatureMessage() => _messageService.ShowError(T("message.featureNotAvailable"));

    private async Task AddPdfsFromDialogAsync()
    {
        if (_isAddingFiles)
        {
            return;
        }

        try
        {
            _isAddingFiles = true;
            AddPdfsCommand.RaiseCanExecuteChanged();

            var paths = _fileDialogService.ShowOpenPdfDialog(_settings.LastInputFolder);
            await AddPdfPathsAsync(paths, CancellationToken.None).ConfigureAwait(true);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            _logger.LogError(exception, "Failed to add PDF files from file dialog.");
            _messageService.ShowError(T("message.filesCouldNotBeAdded"));
        }
        finally
        {
            _isAddingFiles = false;
            AddPdfsCommand.RaiseCanExecuteChanged();
        }
    }

    private static bool TryCreatePdfInputFile(string path, out PdfInputFile inputFile)
    {
        try
        {
            var fullPath = Path.GetFullPath(path);
            inputFile = new PdfInputFile(Path.GetFileName(fullPath), fullPath);
            return true;
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            inputFile = new PdfInputFile(string.Empty, string.Empty);
            return false;
        }
    }

    private bool ContainsSelectedPath(string fullPath) =>
        SelectedFiles.Any(file => string.Equals(file.FullPath, fullPath, StringComparison.OrdinalIgnoreCase));

    private void MoveSelectedFileUp()
    {
        var selectedFile = _selectedFileItems.Single();
        var currentIndex = SelectedFiles.IndexOf(selectedFile);
        SelectedFiles.Move(currentIndex, currentIndex - 1);
        _messageService.ShowStatus(T("status.orderUpdated"));
        RaiseCommandStatesChanged();
    }

    private bool CanMoveSelectedFileUp() =>
        _selectedFileItems.Count == 1 && SelectedFiles.IndexOf(_selectedFileItems[0]) > 0;

    private void MoveSelectedFileDown()
    {
        var selectedFile = _selectedFileItems.Single();
        var currentIndex = SelectedFiles.IndexOf(selectedFile);
        SelectedFiles.Move(currentIndex, currentIndex + 1);
        _messageService.ShowStatus(T("status.orderUpdated"));
        RaiseCommandStatesChanged();
    }

    private bool CanMoveSelectedFileDown() =>
        _selectedFileItems.Count == 1 &&
        SelectedFiles.IndexOf(_selectedFileItems[0]) >= 0 &&
        SelectedFiles.IndexOf(_selectedFileItems[0]) < SelectedFiles.Count - 1;

    private void RemoveSelectedFiles()
    {
        var selectedFiles = _selectedFileItems.ToList();
        foreach (var selectedFile in selectedFiles)
        {
            SelectedFiles.Remove(selectedFile);
        }

        _selectedFileItems.Clear();
        _messageService.ShowSuccess(F("message.filesRemoved", selectedFiles.Count));
        RaiseCommandStatesChanged();
    }

    private bool HasSelectedFiles() => _selectedFileItems.Count > 0;

    private void RequestSelectAll() => SelectAllRequested?.Invoke(this, EventArgs.Empty);

    private void ShowAddFilesResult(int addedCount, int rejectedCount, int duplicateCount, string? singleRejectionMessageKey)
    {
        if (addedCount > 0)
        {
            _messageService.ShowSuccess(F("message.filesAdded", addedCount, rejectedCount, duplicateCount));
            return;
        }

        if (rejectedCount == 1 && duplicateCount == 0 && !string.IsNullOrWhiteSpace(singleRejectionMessageKey))
        {
            _messageService.ShowError(T(singleRejectionMessageKey));
            return;
        }

        if (rejectedCount > 0)
        {
            _messageService.ShowError(F("message.filesRejected", rejectedCount));
            return;
        }

        if (duplicateCount > 0)
        {
            _messageService.ShowSuccess(F("message.duplicateFilesSkipped", duplicateCount));
        }
    }

    private void OnMessageChanged(object? sender, UserMessageChangedEventArgs e)
    {
        StatusText = e.StatusText;
        MessageText = e.MessageText;
        MessageKind = e.MessageKind;
    }

    private string T(string key) => _localizationService.GetString(key);

    private string F(string key, params object[] args) => string.Format(CultureInfo.CurrentCulture, T(key), args);

    private void RaiseCommandStatesChanged()
    {
        MoveUpCommand.RaiseCanExecuteChanged();
        MoveDownCommand.RaiseCanExecuteChanged();
        RemoveCommand.RaiseCanExecuteChanged();
        SelectAllCommand.RaiseCanExecuteChanged();
    }

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
