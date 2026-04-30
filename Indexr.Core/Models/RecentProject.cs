using System;
using System.Collections.Generic;
using System.Text;

namespace Indexr.Core.Models
{
    public class RecentProject
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public DateTime LastOpened { get; set; }
    }
}
