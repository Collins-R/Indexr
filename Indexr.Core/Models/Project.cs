using CommunityToolkit.Mvvm.ComponentModel;
using Indexr.Core.Enums;
using System.Collections.ObjectModel;

namespace Indexr.Core.Models
{
    public partial class Project : ObservableObject
    {
        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _version = string.Empty;

        [ObservableProperty]
        private string _rootFolderPath = string.Empty;

        [ObservableProperty]
        private string _outputPath = string.Empty;

        [ObservableProperty]
        private string _projectFilePath = string.Empty;

        [ObservableProperty]
        private DateTime _lastGenerated;

        public ObservableCollection<ExclusionRule> ExclusionRules { get; set; } = new();
        public ObservableCollection<InclusionRule> InclusionRules { get; set; } = new();
    }
}