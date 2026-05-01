using System;
using System.Collections.Generic;
using System.Text;
using Indexr.Core.Enums;
using Indexr.Core.Models;
using System.Text.RegularExpressions;

namespace Indexr.Core.Services
{
    public class FolderScannerService
    {
        private readonly UncPathService _uncPathService;

        public FolderScannerService(UncPathService uncPathService)
        {
            _uncPathService = uncPathService;
        }

        public (List<IndexGroup> Groups, List<IndexInclusionGroup> InclusionGroups)
            Scan(string rootPath, List<ExclusionRule> exclusionRules,
                 List<InclusionRule> inclusionRules)
        {
            var groups = new List<IndexGroup>();
            var inclusionGroups = new List<IndexInclusionGroup>();

            if (!Directory.Exists(rootPath))
                return (groups, inclusionGroups);

            // Handle root level files first
            var rootFiles = Directory.GetFiles(rootPath);
            if (rootFiles.Length > 0)
            {
                var rootGroup = new IndexGroup
                {
                    FolderName = new DirectoryInfo(rootPath).Name,
                    FolderPath = rootPath,
                    UncFolderPath = _uncPathService.Convert(rootPath),
                    Entries = rootFiles
                        .Select(f => CreateIndexEntry(f))
                        .ToList()
                };
                groups.Add(rootGroup);
            }

            // Get all subdirectories sorted by full path
            var allDirectories = Directory.GetDirectories(rootPath, "*",
                SearchOption.AllDirectories)
                .OrderBy(d => d)
                .ToList();

            foreach (var dirPath in allDirectories)
            {
                // Check inclusion rules first — inclusions always win
                var inclusionRule = GetMatchingInclusionRule(dirPath, inclusionRules);
                if (inclusionRule != null)
                {
                    inclusionGroups.Add(new IndexInclusionGroup
                    {
                        FolderPath = dirPath,
                        UncFolderPath = _uncPathService.Convert(dirPath),
                        DisplayName = inclusionRule.DisplayName ??
                            new DirectoryInfo(dirPath).Name
                    });
                    continue;
                }

                // Check exclusion rules
                if (IsExcluded(dirPath, exclusionRules))
                    continue;

                // Scan folder contents
                var files = Directory.GetFiles(dirPath);
                if (files.Length == 0)
                    continue;

                var group = new IndexGroup
                {
                    FolderName = new DirectoryInfo(dirPath).Name,
                    FolderPath = dirPath,
                    UncFolderPath = _uncPathService.Convert(dirPath),
                    Entries = files
                        .Select(f => CreateIndexEntry(f))
                        .ToList()
                };

                groups.Add(group);
            }

            return (groups, inclusionGroups);
        }

        private IndexEntry CreateIndexEntry(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return new IndexEntry
            {
                FileName = fileInfo.Name,
                FilePath = filePath,
                UncPath = _uncPathService.Convert(filePath),
                LastModified = fileInfo.LastWriteTime
            };
        }

        private InclusionRule? GetMatchingInclusionRule(string dirPath,
            List<InclusionRule> inclusionRules)
        {
            return inclusionRules.FirstOrDefault(r =>
                dirPath.Equals(r.FolderPath, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsExcluded(string dirPath, List<ExclusionRule> exclusionRules)
        {
            var dirName = new DirectoryInfo(dirPath).Name;

            foreach (var rule in exclusionRules.Where(r => r.Type == FilterRuleType.Folder))
            {
                if (rule.PatternType == FilterPatternType.PlainText)
                {
                    if (dirName.Equals(rule.Pattern, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                else if (rule.PatternType == FilterPatternType.Regex)
                {
                    if (Regex.IsMatch(dirName, rule.Pattern, RegexOptions.IgnoreCase))
                        return true;
                }
            }

            return false;
        }
    }
}
