using System;
using System.Collections.Generic;
using System.Text;

namespace Indexr.Core.Services
{
    public class UncPathService
    {
        public string Convert(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            try
            {
                var driveRoot = Path.GetPathRoot(path);

                if (string.IsNullOrWhiteSpace(driveRoot))
                    return path;

                var driveInfo = new DriveInfo(driveRoot);

                if (driveInfo.DriveType == DriveType.Network)
                {
                    var uncRoot = GetUncPath(driveRoot.TrimEnd('\\'));

                    if (!string.IsNullOrWhiteSpace(uncRoot))
                        return path.Replace(driveRoot.TrimEnd('\\'), uncRoot);
                }
            }
            catch
            {
                // If conversion fails return original path
            }

            return path;
        }

        private string GetUncPath(string driveLetter)
        {
            try
            {
                using var key = Microsoft.Win32.Registry.LocalMachine
                    .OpenSubKey($@"SYSTEM\CurrentControlSet\Services\lanmanworkstation\parameters");

                // Fallback to mapped drive lookup via WMI or return as is
                var drives = System.IO.DriveInfo.GetDrives();

                foreach (var drive in drives)
                {
                    if (drive.Name.TrimEnd('\\').Equals(driveLetter,
                        StringComparison.OrdinalIgnoreCase) &&
                        drive.DriveType == DriveType.Network)
                    {
                        return drive.RootDirectory.FullName;
                    }
                }
            }
            catch
            {
                // Return null if lookup fails
            }

            return null;
        }
    }
}
