using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using PdfMerge.Application.Localization;
using PdfMerge.Application.Merging;
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
    private readonly IConfirmationService _confirmationService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IPdfMergeService _pdfMergeService;
    private readonly IPdfInputValidator _pdfInputValidator;
    private readonly IApplicationLifetime _applicationLifetime;
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly List<SelectedPdfViewModel> _selectedFileItems = [];
    private string _statusText = string.Empty;
    private UserMessageKind _messageKind;
    private FlowDirection _flowDirection = FlowDirection.LeftToRight;
    private AppSettings _settings = AppSettings.CreateDefault();
    private bool _isAddingFiles;
    private bool _isMerging;

    public MainWindowViewModel(
        ISettingsService settingsService,
        ILocalizationService localizationService,
        IMessageService messageService,
        IConfirmationService confirmationService,
        IFileDialogService fileDialogService,
        IPdfMergeService pdfMergeService,
        IPdfInputValidator pdfInputValidator,
        IApplicationLifetime applicationLifetime,
        ILogger<MainWindowViewModel> logger)
    {
        _settingsService = settingsService;
        _localizationService = localizationService;
        _messageService = messageService;
        _confirmationService = confirmationService;
        _fileDialogService = fileDialogService;
        _pdfMergeService = pdfMergeService;
        _pdfInputValidator = pdfInputValidator;
        _applicationLifetime = applicationLifetime;
        _logger = logger;

        SelectedFiles = new ObservableCollection<SelectedPdfViewModel>();
        AddPdfsCommand = new RelayCommand(() => _ = AddPdfsFromDialogAsync(), () => !_isAddingFiles && !_isMerging);
        ExitCommand = new RelayCommand(_applicationLifetime.Shutdown);
        SetEnglishLanguageCommand = new RelayCommand(() => _ = SetLanguageAsync(SupportedLanguages.English));
        SetHebrewLanguageCommand = new RelayCommand(() => _ = SetLanguageAsync(SupportedLanguages.Hebrew));
        MoveUpCommand = new RelayCommand(MoveSelectedFileUp, CanMoveSelectedFileUp);
        MoveDownCommand = new RelayCommand(MoveSelectedFileDown, CanMoveSelectedFileDown);
        RemoveCommand = new RelayCommand(RemoveSelectedFiles, HasSelectedFiles);
        MoveFileUpCommand = new RelayCommand(MoveFileUp, CanMoveFileUp);
        MoveFileDownCommand = new RelayCommand(MoveFileDown, CanMoveFileDown);
        RemoveFileCommand = new RelayCommand(RemoveFile, CanRemoveFile);
        SelectAllCommand = new RelayCommand(RequestSelectAll, () => !_isMerging && SelectedFiles.Count > 0);
        MergeCommand = new RelayCommand(() => _ = MergeAsync(), CanMerge);
        OpenGitHubProjectCommand = new RelayCommand(ShowDeferredFeatureMessage, () => false);
        AboutCommand = new RelayCommand(() => _messageService.ShowSuccess(T("message.about")));

        _messageService.MessageChanged += OnMessageChanged;
        SelectedFiles.CollectionChanged += (_, _) => RaiseCommandStatesChanged();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public event EventHandler? SelectAllRequested;

    public ObservableCollection<SelectedPdfViewModel> SelectedFiles { get; }

    public RelayCommand AddPdfsCommand { get; }

    public ICommand ExitCommand { get; }

    public ICommand SetEnglishLanguageCommand { get; }

    public ICommand SetHebrewLanguageCommand { get; }

    public RelayCommand MoveUpCommand { get; }

    public RelayCommand MoveDownCommand { get; }

    public RelayCommand RemoveCommand { get; }

    public RelayCommand MoveFileUpCommand { get; }

    public RelayCommand MoveFileDownCommand { get; }

    public RelayCommand RemoveFileCommand { get; }

    public RelayCommand SelectAllCommand { get; }

    public RelayCommand MergeCommand { get; }

    public ICommand OpenGitHubProjectCommand { get; }

    public ICommand AboutCommand { get; }

    public AppSettings Settings => _settings;

    public string Title => T("app.title");

    public string FileMenuText => T("menu.file");

    public string EditMenuText => T("menu.edit");

    public string HelpMenuText => T("menu.help");

    public string LanguageMenuText => T("menu.language");

    public string AddPdfsText => T("command.addPdfs");

    public string ExitText => T("command.exit");

    public string EnglishLanguageText => T("language.english");

    public string HebrewLanguageText => T("language.hebrew");

    public bool IsEnglishSelected => string.Equals(_settings.Language, SupportedLanguages.English, StringComparison.OrdinalIgnoreCase);

    public bool IsHebrewSelected => string.Equals(_settings.Language, SupportedLanguages.Hebrew, StringComparison.OrdinalIgnoreCase);

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

    public string ActionsHeaderText => T("column.actions");

    public string EmptyListText => T("message.emptyList");

    public string StatusText
    {
        get => _statusText;
        private set => SetField(ref _statusText, value);
    }

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
        ApplyCurrentLanguage();
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
        if (_isMerging)
        {
            _messageService.ShowStatus(T("status.mergeInProgress"));
            return;
        }

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

    private async Task SetLanguageAsync(string language)
    {
        if (!SupportedLanguages.IsSupported(language))
        {
            return;
        }

        if (string.Equals(_settings.Language, language, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _settings.Language = language;
        ApplyCurrentLanguage();
        await SaveSettingsAsync(CancellationToken.None).ConfigureAwait(true);
        _messageService.ShowStatus(T("status.languageChanged"));
    }

    private async Task MergeAsync()
    {
        if (_isMerging)
        {
            return;
        }

        if (SelectedFiles.Count < 2)
        {
            _messageService.ShowError(T("message.mergeRequiresTwoFiles"));
            return;
        }

        var inputFiles = SelectedFiles
            .Select(file => new PdfInputFile(file.FileName, file.FullPath))
            .ToList();

        string? temporaryOutputPath = null;
        SetIsMerging(true);

        try
        {
            var invalidMessageKey = await ValidateMergeInputsAsync(inputFiles, CancellationToken.None).ConfigureAwait(true);
            if (!string.IsNullOrWhiteSpace(invalidMessageKey))
            {
                _messageService.ShowError(T(invalidMessageKey));
                return;
            }

            var outputPath = _fileDialogService.ShowSavePdfDialog(_settings.LastOutputFolder);
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                return;
            }

            if (!TryNormalizeOutputPath(outputPath, out var normalizedOutputPath))
            {
                _messageService.ShowError(T("message.outputPathInvalid"));
                return;
            }

            if (inputFiles.Any(file => string.Equals(file.FullPath, normalizedOutputPath, StringComparison.OrdinalIgnoreCase)))
            {
                _messageService.ShowError(T("message.outputCannotOverwriteInput"));
                return;
            }

            var outputDirectory = Path.GetDirectoryName(normalizedOutputPath);
            if (string.IsNullOrWhiteSpace(outputDirectory) || !Directory.Exists(outputDirectory))
            {
                _messageService.ShowError(T("message.outputDirectoryMissing"));
                return;
            }

            if (File.Exists(normalizedOutputPath) && !_confirmationService.ConfirmOverwrite(normalizedOutputPath))
            {
                _messageService.ShowStatus(T("status.mergeCanceled"));
                return;
            }

            temporaryOutputPath = CreateTemporaryOutputPath(outputDirectory, normalizedOutputPath);
            _messageService.ShowStatus(T("status.mergeRunning"));
            _logger.LogInformation("Merge started for {InputFileCount} PDF files.", inputFiles.Count);

            var progress = new Progress<MergeProgress>(mergeProgress =>
            {
                _messageService.ShowStatus(F("status.mergeProgress", mergeProgress.CompletedFiles, mergeProgress.TotalFiles));
            });

            await _pdfMergeService
                .MergeAsync(new MergeRequest(inputFiles, temporaryOutputPath), progress, CancellationToken.None)
                .ConfigureAwait(true);

            File.Move(temporaryOutputPath, normalizedOutputPath, overwrite: true);

            _settings.LastOutputFolder = outputDirectory;
            await SaveSettingsAsync(CancellationToken.None).ConfigureAwait(true);

            _logger.LogInformation("Merge completed for {InputFileCount} PDF files.", inputFiles.Count);
            _messageService.ShowSuccess(F("message.mergeSucceeded", inputFiles.Count));
        }
        catch (PdfMergeException exception)
        {
            TryDeleteTemporaryFile(temporaryOutputPath);
            _logger.LogError(exception, "Merge failed with known PDF merge error.");
            _messageService.ShowError(T(exception.MessageKey));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            TryDeleteTemporaryFile(temporaryOutputPath);
            _logger.LogError(exception, "Merge failed because output could not be written.");
            _messageService.ShowError(T("message.outputWriteFailed"));
        }
        finally
        {
            SetIsMerging(false);
        }
    }

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
        MoveFileUp(_selectedFileItems.Single());
    }

    private bool CanMoveSelectedFileUp() =>
        _selectedFileItems.Count == 1 && CanMoveFileUp(_selectedFileItems[0]);

    private void MoveSelectedFileDown()
    {
        MoveFileDown(_selectedFileItems.Single());
    }

    private bool CanMoveSelectedFileDown() =>
        _selectedFileItems.Count == 1 && CanMoveFileDown(_selectedFileItems[0]);

    private void MoveFileUp(object? parameter)
    {
        if (parameter is not SelectedPdfViewModel selectedFile)
        {
            return;
        }

        var currentIndex = SelectedFiles.IndexOf(selectedFile);
        if (currentIndex <= 0)
        {
            return;
        }

        SelectedFiles.Move(currentIndex, currentIndex - 1);
        _messageService.ShowStatus(T("status.orderUpdated"));
        RaiseCommandStatesChanged();
    }

    private bool CanMoveFileUp(object? parameter) =>
        !_isMerging && parameter is SelectedPdfViewModel selectedFile && SelectedFiles.IndexOf(selectedFile) > 0;

    private void MoveFileDown(object? parameter)
    {
        if (parameter is not SelectedPdfViewModel selectedFile)
        {
            return;
        }

        var currentIndex = SelectedFiles.IndexOf(selectedFile);
        if (currentIndex < 0 || currentIndex >= SelectedFiles.Count - 1)
        {
            return;
        }

        SelectedFiles.Move(currentIndex, currentIndex + 1);
        _messageService.ShowStatus(T("status.orderUpdated"));
        RaiseCommandStatesChanged();
    }

    private bool CanMoveFileDown(object? parameter) =>
        !_isMerging &&
        parameter is SelectedPdfViewModel selectedFile &&
        SelectedFiles.IndexOf(selectedFile) >= 0 &&
        SelectedFiles.IndexOf(selectedFile) < SelectedFiles.Count - 1;

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

    private void RemoveFile(object? parameter)
    {
        if (parameter is not SelectedPdfViewModel selectedFile || !SelectedFiles.Contains(selectedFile))
        {
            return;
        }

        SelectedFiles.Remove(selectedFile);
        _selectedFileItems.Remove(selectedFile);
        _messageService.ShowSuccess(F("message.filesRemoved", 1));
        RaiseCommandStatesChanged();
    }

    private bool CanRemoveFile(object? parameter) =>
        !_isMerging && parameter is SelectedPdfViewModel selectedFile && SelectedFiles.Contains(selectedFile);

    private bool HasSelectedFiles() => !_isMerging && _selectedFileItems.Count > 0;

    private void RequestSelectAll() => SelectAllRequested?.Invoke(this, EventArgs.Empty);

    private bool CanMerge() => !_isMerging && SelectedFiles.Count >= 2;

    private async Task<string?> ValidateMergeInputsAsync(IReadOnlyList<PdfInputFile> inputFiles, CancellationToken cancellationToken)
    {
        foreach (var inputFile in inputFiles)
        {
            var validationResult = await _pdfInputValidator.ValidateAsync(inputFile, cancellationToken).ConfigureAwait(true);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Merge input validation failed for file {FileName} with message key {MessageKey}.", inputFile.FileName, validationResult.MessageKey);
                return validationResult.MessageKey;
            }
        }

        return null;
    }

    private static bool TryNormalizeOutputPath(string outputPath, out string normalizedOutputPath)
    {
        try
        {
            normalizedOutputPath = Path.GetFullPath(outputPath);
            if (!string.Equals(Path.GetExtension(normalizedOutputPath), ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                normalizedOutputPath = Path.ChangeExtension(normalizedOutputPath, ".pdf");
            }

            return true;
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            normalizedOutputPath = string.Empty;
            return false;
        }
    }

    private static string CreateTemporaryOutputPath(string outputDirectory, string outputPath)
    {
        var fileName = Path.GetFileNameWithoutExtension(outputPath);
        return Path.Combine(outputDirectory, $"{fileName}.{Guid.NewGuid():N}.tmp.pdf");
    }

    private static void TryDeleteTemporaryFile(string? temporaryOutputPath)
    {
        if (string.IsNullOrWhiteSpace(temporaryOutputPath))
        {
            return;
        }

        try
        {
            if (File.Exists(temporaryOutputPath))
            {
                File.Delete(temporaryOutputPath);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private void SetIsMerging(bool isMerging)
    {
        _isMerging = isMerging;
        RaiseCommandStatesChanged();
    }

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
        MessageKind = e.MessageKind;
    }

    private string T(string key) => _localizationService.GetString(key);

    private string F(string key, params object[] args) => string.Format(CultureInfo.CurrentCulture, T(key), args);

    private void RaiseCommandStatesChanged()
    {
        AddPdfsCommand.RaiseCanExecuteChanged();
        MoveUpCommand.RaiseCanExecuteChanged();
        MoveDownCommand.RaiseCanExecuteChanged();
        RemoveCommand.RaiseCanExecuteChanged();
        MoveFileUpCommand.RaiseCanExecuteChanged();
        MoveFileDownCommand.RaiseCanExecuteChanged();
        RemoveFileCommand.RaiseCanExecuteChanged();
        SelectAllCommand.RaiseCanExecuteChanged();
        MergeCommand.RaiseCanExecuteChanged();
    }

    private void RaiseLocalizedPropertiesChanged()
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(FileMenuText));
        OnPropertyChanged(nameof(EditMenuText));
        OnPropertyChanged(nameof(HelpMenuText));
        OnPropertyChanged(nameof(LanguageMenuText));
        OnPropertyChanged(nameof(AddPdfsText));
        OnPropertyChanged(nameof(ExitText));
        OnPropertyChanged(nameof(EnglishLanguageText));
        OnPropertyChanged(nameof(HebrewLanguageText));
        OnPropertyChanged(nameof(IsEnglishSelected));
        OnPropertyChanged(nameof(IsHebrewSelected));
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
        OnPropertyChanged(nameof(ActionsHeaderText));
        OnPropertyChanged(nameof(EmptyListText));
    }

    private void ApplyCurrentLanguage()
    {
        _localizationService.SetLanguage(_settings.Language);
        FlowDirection = _localizationService.Direction == CultureDirection.RightToLeft
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;
        RaiseLocalizedPropertiesChanged();
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
