using System;
using System.Collections.Generic;
using System.Text;

namespace Indexr.Core.Models
{
    public class IndexEntry
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string UncPath { get; set; }
        public DateTime LastModified { get; set; }
    }
}
