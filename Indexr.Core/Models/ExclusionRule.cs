using System;
using System.Collections.Generic;
using System.Text;
using Indexr.Core.Enums;

namespace Indexr.Core.Models
{
    public class ExclusionRule
    {
        public string Pattern { get; set; }
        public FilterRuleType Type { get; set; }
        public FilterPatternType PatternType { get; set; }
    }
}
