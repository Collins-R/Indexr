
namespace Indexr.Core.Models
{
    public class Project
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string RootFolderPath { get; set; }
        public string OutputPath { get; set; }
        public string ProjectFilePath { get; set; }
        public List<ExclusionRule> ExclusionRules { get; set; } = new();
        public List<InclusionRule> InclusionRules { get; set; } = new();
        public DateTime LastGenerated { get; set; }
    }
}