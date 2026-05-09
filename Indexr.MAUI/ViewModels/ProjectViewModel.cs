using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Indexr.Core.Enums;
using Indexr.Core.Models;
using Indexr.Core.Services;

namespace Indexr.MAUI.ViewModels
{
    [QueryProperty(nameof(ProjectFilePath), "ProjectFilePath")]
    public partial class ProjectViewModel : ObservableObject
    {
        private readonly FolderScannerService _folderScannerService;
        private readonly DocumentGeneratorService _documentGeneratorService;
        private readonly ProjectFileService _projectFileService;
        private readonly RecentProjectsService _recentProjectsService;
        private readonly VersionIncrementService _versionIncrementService;

        [ObservableProperty]
        private Project _project = new();

        [ObservableProperty]
        private bool _isGenerating;

        [ObservableProperty]
        private string _statusMessage = string.Empty;

        [ObservableProperty]
        private bool _hasStatusMessage;

        private string _projectFilePath = string.Empty;
        public string ProjectFilePath
        {
            get => _projectFilePath;
            set
            {
                _projectFilePath = value;
                if (!string.IsNullOrWhiteSpace(value))
                    LoadProject(value);
            }
        }

        public List<string> FilterRuleTypes { get; } =
            Enum.GetNames(typeof(FilterRuleType)).ToList();

        public List<string> FilterPatternTypes { get; } =
            Enum.GetNames(typeof(FilterPatternType)).ToList();

        public bool HasNoExclusionRules => Project.ExclusionRules.Count == 0;
        public bool HasNoInclusionRules => Project.InclusionRules.Count == 0;


        public ProjectViewModel(
            FolderScannerService folderScannerService,
            DocumentGeneratorService documentGeneratorService,
            ProjectFileService projectFileService,
            RecentProjectsService recentProjectsService,
            VersionIncrementService versionIncrementService)
        {
            _folderScannerService = folderScannerService;
            _documentGeneratorService = documentGeneratorService;
            _projectFileService = projectFileService;
            _recentProjectsService = recentProjectsService;
            _versionIncrementService = versionIncrementService;
        }

        [RelayCommand]
        private async Task BrowseRootFolder()
        {
            var result = await FolderPicker.Default.PickAsync();
            if (result.IsSuccessful)
                Project.RootFolderPath = result.Folder.Path;
        }

        [RelayCommand]
        private async Task BrowseOutputFolder()
        {
            var result = await FolderPicker.Default.PickAsync();
            if (result.IsSuccessful)
                Project.OutputPath = result.Folder.Path;
        }

        [RelayCommand]
        private async Task SaveProject()
        {
            var result = await FileSaver.Default.SaveAsync(
                $"{Project.Name}.indexr",
                new MemoryStream(),
                CancellationToken.None);

            if (result.IsSuccessful)
            {
                _projectFileService.Save(Project, result.FilePath);
                _recentProjectsService.AddRecentProject(Project.Name, result.FilePath);
                await ShowStatus("Project saved successfully");
            }
        }

        [RelayCommand]
        private async Task Generate()
        {
            if (string.IsNullOrWhiteSpace(Project.RootFolderPath))
            {
                await ShowStatus("Please select a root folder first");
                return;
            }

            if (string.IsNullOrWhiteSpace(Project.OutputPath))
            {
                await ShowStatus("Please select an output location first");
                return;
            }

            try
            {
                IsGenerating = true;
                await ShowStatus("Scanning folders...");

                var (groups, inclusionGroups) = await Task.Run(() =>
                    _folderScannerService.Scan(
                        Project.RootFolderPath,
                        Project.ExclusionRules,
                        Project.InclusionRules));

                await ShowStatus("Generating document...");

                var fileName = $"{Project.Name} Document Index v{Project.Version}.pdf";
                var outputPath = Path.Combine(Project.OutputPath, fileName);

                await Task.Run(() =>
                    _documentGeneratorService.Generate(
                        Project, groups, inclusionGroups, outputPath));

                Project.LastGenerated = DateTime.Now;

                if (!string.IsNullOrWhiteSpace(Project.ProjectFilePath))
                    _projectFileService.Save(Project, Project.ProjectFilePath);

                var open = await Shell.Current.DisplayAlert(
                    "Generation Complete",
                    $"Document index generated successfully.\n\nWould you like to open it?",
                    "Open", "Close");

                if (open)
                {
                    await Launcher.Default.OpenAsync(
                        new OpenFileRequest
                        {
                            File = new ReadOnlyFile(outputPath)
                        });
                }
            }
            catch (Exception ex)
            {
                await ShowStatus($"Error: {ex.Message}");
            }
            finally
            {
                IsGenerating = false;
            }
        }

        [RelayCommand]
        private void AddExclusionRule()
        {
            Project.ExclusionRules.Add(new ExclusionRule
            {
                Pattern = string.Empty,
                Type = FilterRuleType.Folder,
                PatternType = FilterPatternType.PlainText
            });
            OnPropertyChanged(nameof(HasNoExclusionRules));
        }

        [RelayCommand]
        private void RemoveExclusionRule(ExclusionRule rule)
        {
            Project.ExclusionRules.Remove(rule);
            OnPropertyChanged(nameof(HasNoExclusionRules));
        }

        [RelayCommand]
        private void AddInclusionRule()
        {
            Project.InclusionRules.Add(new InclusionRule
            {
                FolderPath = string.Empty,
                DisplayName = string.Empty
            });
            OnPropertyChanged(nameof(HasNoInclusionRules));
        }

        [RelayCommand]
        private void RemoveInclusionRule(InclusionRule rule)
        {
            Project.InclusionRules.Remove(rule);
            OnPropertyChanged(nameof(HasNoInclusionRules));
        }

        private async Task ShowStatus(string message)
        {
            StatusMessage = message;
            HasStatusMessage = true;
            await Task.Delay(3000);
            HasStatusMessage = false;
        }

        private void LoadProject(string filePath)
        {
            var project = _projectFileService.Load(filePath);
            if (project != null)
            {
                project.Version = _versionIncrementService.Increment(project.Version);
                Project = project;
            }
        }
    }
}