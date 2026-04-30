using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Indexr.Core.Models;

namespace Indexr.Core.Services
{
    public class RecentProjectsService
    {
        private readonly string _settingsPath;
        private readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true
        };
        private const int MaxRecentProjects = 5;

        public RecentProjectsService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = Path.Combine(appData, "Indexr");
            Directory.CreateDirectory(folder);
            _settingsPath = Path.Combine(folder, "settings.json");
        }

        public AppSettings Load()
        {
            if (!File.Exists(_settingsPath))
                return new AppSettings();

            try
            {
                var json = File.ReadAllText(_settingsPath);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void AddRecentProject(string name, string filePath)
        {
            var settings = Load();

            // Remove if already exists
            settings.RecentProjects.RemoveAll(r =>
                r.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));

            // Add to top of list
            settings.RecentProjects.Insert(0, new RecentProject
            {
                Name = name,
                FilePath = filePath,
                LastOpened = DateTime.Now
            });

            // Trim to max
            if (settings.RecentProjects.Count > MaxRecentProjects)
                settings.RecentProjects = settings.RecentProjects
                    .Take(MaxRecentProjects)
                    .ToList();

            Save(settings);
        }

        private void Save(AppSettings settings)
        {
            var json = JsonSerializer.Serialize(settings, _options);
            File.WriteAllText(_settingsPath, json);
        }
    }
}
