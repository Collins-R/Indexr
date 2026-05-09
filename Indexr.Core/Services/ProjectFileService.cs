using System.Text.Json;
using System.Text.Json.Serialization;
using Indexr.Core.Models;

namespace Indexr.Core.Services
{
    public class ProjectFileService
    {
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public void Save(Project project, string filePath)
        {
            var json = JsonSerializer.Serialize(project, _options);
            File.WriteAllText(filePath, json);
            project.ProjectFilePath = filePath;
        }

        public Project? Load(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            var json = File.ReadAllText(filePath);
            var migrated = MigrateEnums(json);
            return JsonSerializer.Deserialize<Project>(migrated, _options);
        }

        private string MigrateEnums(string json)
        {
            json = json.Replace("\"Type\": 0", "\"Type\": \"Folder\"");
            json = json.Replace("\"Type\": 1", "\"Type\": \"File\"");
            json = json.Replace("\"PatternType\": 0", "\"PatternType\": \"PlainText\"");
            json = json.Replace("\"PatternType\": 1", "\"PatternType\": \"Regex\"");
            return json;
        }
    }
}