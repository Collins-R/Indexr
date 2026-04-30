using System;
using System.Collections.Generic;
using System.Text;

namespace Indexr.Core.Services
{
    public class VersionIncrementService
    {
        public string Increment(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return version;

            var parts = version.Split('.');

            for (int i = parts.Length - 1; i >= 0; i--)
            {
                if (int.TryParse(parts[i], out int number))
                {
                    parts[i] = (number + 1).ToString();
                    return string.Join('.', parts);
                }
            }

            // No numeric segment found, return as is
            return version;
        }
    }
}
