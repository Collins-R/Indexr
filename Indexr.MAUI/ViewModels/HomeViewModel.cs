using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Indexr.Core.Models;
using Indexr.Core.Services;

namespace Indexr.MAUI.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly RecentProjectsService _recentProjectsService;

        [ObservableProperty]
        private List<RecentProject> _recentProjects = new();

        public HomeViewModel(RecentProjectsService recentProjectsService)
        {
            _recentProjectsService = recentProjectsService;
            LoadRecentProjects();
        }

        private void LoadRecentProjects()
        {
            var settings = _recentProjectsService.Load();
            RecentProjects = settings.RecentProjects;
        }

        [RelayCommand]
        private async Task NewProject()
        {
            await Shell.Current.GoToAsync("//ProjectPage");
        }

        [RelayCommand]
        private async Task OpenRecentProject(RecentProject project)
        {
            await Shell.Current.GoToAsync("//ProjectPage", new Dictionary<string, object>
            {
                { "Project", project }
            });
        }
    }
}