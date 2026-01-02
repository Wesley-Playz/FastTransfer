using System.Runtime.InteropServices;
using System.Text;

namespace FastTransfer
{
    public static class Shell32Helper
    {
        [ComImport]
        [Guid("9BA05972-F6A8-11CF-A442-00A0C90A8F39")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellWindows
        {
            [return: MarshalAs(UnmanagedType.IDispatch)]
            object Item([In] object index);
            
            int Count { [return: MarshalAs(UnmanagedType.I4)] get; }
        }

        [ComImport]
        [Guid("D8F015C0-C278-11CE-A49E-444553540000")]
        [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
        private interface IWebBrowser2
        {
            void GoBack();
            void GoForward();
            void GoHome();
            void GoSearch();
            void Navigate([In] string Url, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers);
            void Refresh();
            void Refresh2([In] ref object level);
            void Stop();
            
            [return: MarshalAs(UnmanagedType.IDispatch)]
            object Application { get; }
            
            [return: MarshalAs(UnmanagedType.IDispatch)]
            object Parent { get; }
            
            [return: MarshalAs(UnmanagedType.IDispatch)]
            object Container { get; }
            
            [return: MarshalAs(UnmanagedType.IDispatch)]
            object Document { get; }
            
            bool TopLevelContainer { get; }
            string Type { get; }
            int Left { get; set; }
            int Top { get; set; }
            int Width { get; set; }
            int Height { get; set; }
            string LocationName { get; }
            string LocationURL { get; }
            bool Busy { get; }
            void Quit();
            void ClientToWindow(out int pcx, out int pcy);
            void PutProperty([In] string property, [In] object vtValue);
            object GetProperty([In] string property);
            string Name { get; }
            int HWND { get; }
            string FullName { get; }
            string Path { get; }
            bool Visible { get; set; }
            bool StatusBar { get; set; }
            string StatusText { get; set; }
            int ToolBar { get; set; }
            bool MenuBar { get; set; }
            bool FullScreen { get; set; }
        }

        public static List<string> GetOpenExplorerPaths()
        {
            var paths = new List<string>();

            try
            {
                var shellWindowsType = Type.GetTypeFromCLSID(new Guid("9BA05972-F6A8-11CF-A442-00A0C90A8F39"));
                if (shellWindowsType == null)
                    return paths;

                dynamic? shellWindows = Activator.CreateInstance(shellWindowsType);
                if (shellWindows == null)
                    return paths;

                try
                {
                    int count = shellWindows.Count;
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            dynamic? window = shellWindows.Item(i);
                            if (window == null)
                                continue;

                            try
                            {
                                string? fullName = window.FullName as string;
                                string? locationURL = window.LocationURL as string;

                                if (!string.IsNullOrEmpty(fullName) && 
                                    !string.IsNullOrEmpty(locationURL) &&
                                    fullName.EndsWith("explorer.exe", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (Uri.TryCreate(locationURL, UriKind.Absolute, out Uri? uri))
                                    {
                                        string localPath = Uri.UnescapeDataString(uri.LocalPath);
                                        if (Directory.Exists(localPath) && !paths.Contains(localPath))
                                        {
                                            paths.Add(localPath);
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                if (window != null && Marshal.IsComObject(window))
                                    Marshal.ReleaseComObject(window);
                            }
                        }
                        catch
                        {
                            // Skip this window if there's an error
                            continue;
                        }
                    }
                }
                finally
                {
                    if (shellWindows != null && Marshal.IsComObject(shellWindows))
                        Marshal.ReleaseComObject(shellWindows);
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
