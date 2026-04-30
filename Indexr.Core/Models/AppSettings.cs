using System;
using System.Collections.Generic;
using System.Text;

namespace Indexr.Core.Models
{
    public class AppSettings
    {
        public List<RecentProject> RecentProjects { get; set; } = new();
    }
}
