namespace FastTransfer
{
    public enum FileConflictAction
    {
        Replace,
        Skip,
        Cancel
    }

    public partial class FileConflictDialog : Form
    {
        public FileConflictAction Action { get; private set; }
        public bool ApplyToAll { get; private set; }

        public FileConflictDialog(string fileName, string sourcePath, string destPath)
        {
            InitializeComponent();
            this.Text = "File Already Exists";
            
            lblMessage.Text = $"The file '{fileName}' already exists in the destination folder.";
            
            // Get file info
            if (File.Exists(sourcePath))
            {
                var sourceInfo = new FileInfo(sourcePath);
                lblSourceInfo.Text = $"Size: {FormatBytes(sourceInfo.Length)}\nModified: {sourceInfo.LastWriteTime:g}";
            }
            
            if (File.Exists(destPath))
            {
                var destInfo = new FileInfo(destPath);
                lblDestInfo.Text = $"Size: {FormatBytes(destInfo.Length)}\nModified: {destInfo.LastWriteTime:g}";
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
