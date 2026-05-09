using Indexr.Core.Enums;

namespace Indexr.Core.Models
{
    public class ExclusionRule
    {
        public string Pattern { get; set; } = string.Empty;
        public FilterRuleType Type { get; set; }
        public FilterPatternType PatternType { get; set; }

        // Helper properties for Picker binding
        public string TypeString
        {
            get => Type.ToString();
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    Type = Enum.Parse<FilterRuleType>(value);
            }
        }

        public string PatternTypeString
        {
            get => PatternType.ToString();
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    PatternType = Enum.Parse<FilterPatternType>(value);
            }
        }
    }
}