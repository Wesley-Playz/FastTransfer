using System.Runtime.InteropServices;
using SHDocVw;

namespace FastTransfer
{
    public static class Shell32Helper
    {
        public static List<string> GetOpenExplorerPaths()
        {
            var paths = new List<string>();

            try
            {
                var shellWindows = new ShellWindows();
                foreach (InternetExplorer window in shellWindows)
                {
                    var fileName = Path.GetFileName(window.FullName);
                    if (fileName != null && fileName.Equals("explorer.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        var locationUrl = window.LocationURL;
                        if (!string.IsNullOrEmpty(locationUrl))
                        {
                            var uri = new Uri(locationUrl);
                            var localPath = Uri.UnescapeDataString(uri.LocalPath);
                            if (Directory.Exists(localPath))
                            {
                                paths.Add(localPath);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting explorer windows: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return paths;
        }
    }
}
