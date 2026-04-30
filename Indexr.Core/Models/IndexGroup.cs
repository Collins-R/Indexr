using System;
using System.Collections.Generic;
using System.Text;

namespace Indexr.Core.Models
{
    public class IndexGroup
    {
        public string FolderName { get; set; }
        public string FolderPath { get; set; }
        public string UncFolderPath { get; set; }
        public List<IndexEntry> Entries { get; set; } = new();
    }
}
