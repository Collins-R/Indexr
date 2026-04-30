using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Indexr.Core.Models;

namespace Indexr.Core.Services
{
    public class ProjectFileService
    {
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
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
            return JsonSerializer.Deserialize<Project>(json);
        }
    }
}
