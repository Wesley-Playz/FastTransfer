using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FastTransfer
{
    public partial class TransferProgressForm : Form
    {
        private readonly string sourcePath;
        private readonly string destFolder;
        private readonly bool isFile;
        private readonly bool isMoveOperation;
        private Process robocopyProcess;
        private CancellationTokenSource cancellationTokenSource;
        private FileConflictAction? applyToAllAction = null;
        private Stopwatch transferStopwatch;
        private long lastBytesTransferred;
        private DateTime lastSpeedUpdate;
        private string currentSpeed = "";
        private string currentSpeedBits = "";
        private long totalFolderSize;
        private long totalBytesTransferred;
        private int totalFiles;
        private int filesTransferred;

        public TransferProgressForm(string source, string destination, bool isFile, bool isMove)
        {
            InitializeComponent();

            this.sourcePath = source;
            this.destFolder = destination;
            this.isFile = isFile;
            this.isMoveOperation = isMove;

            lblSource.Text = TruncatePath(source, 70);
            lblDest.Text = TruncatePath(destination, 70);
            lblOperation.Text = isMove ? "Move" : "Copy";

            cancellationTokenSource = new CancellationTokenSource();
            transferStopwatch = new Stopwatch();

            // Start the operation immediately
            this.Load += (s, e) => StartTransfer();
        }

        private string TruncatePath(string path, int maxLength)
        {
            if (path.Length <= maxLength) return path;

            var parts = path.Split(Path.DirectorySeparatorChar);
            if (parts.Length <= 2) return path;

            return parts[0] + Path.DirectorySeparatorChar + "..." + Path.DirectorySeparatorChar +
                   string.Join(Path.DirectorySeparatorChar, parts.Skip(parts.Length - 2));
        }

        private void StartTransfer()
        {
            transferStopwatch.Start();
            lastSpeedUpdate = DateTime.Now;
            lastBytesTransferred = 0;
            currentSpeed = "";

            if (isFile)
            {
                StartFileCopy(sourcePath, destFolder, cancellationTokenSource.Token);
            }
            else
            {
                // Start transfer immediately and calculate size in parallel
                StartFolderCopy(sourcePath, destFolder, cancellationTokenSource.Token);
            }
        }

        private void CalculateFolderSize(string folderPath, CancellationToken ct)
        {
            try
            {
                var dirInfo = new DirectoryInfo(folderPath);
                var files = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories);

                long calculatedSize = 0;
                int calculatedFiles = 0;

                foreach (var file in files)
                {
                    if (ct.IsCancellationRequested)
                        break;

                    calculatedSize += file.Length;
                    calculatedFiles++;

                    // Update totals periodically (every 100 files)
                    if (calculatedFiles % 100 == 0)
                    {
                        totalFolderSize = calculatedSize;
                        totalFiles = calculatedFiles;
                    }
                }

                // Final update
                if (!ct.IsCancellationRequested)
                {
                    totalFolderSize = calculatedSize;
                    totalFiles = calculatedFiles;
                }
            }
            catch (Exception ex)
            {
                // If we can't calculate size, we'll just show progress without percentage
                totalFolderSize = 0;
                totalFiles = 0;
            }
        }

        private FileConflictAction HandleFileConflict(string fileName, string sourcePath, string destPath)
        {
            // If user already chose "Apply to all", use that action
            if (applyToAllAction.HasValue)
            {
                return applyToAllAction.Value;
            }

            // Pause stopwatch during dialog
            transferStopwatch.Stop();

            // Show conflict dialog on UI thread
            FileConflictAction action = FileConflictAction.Cancel;
            bool applyToAll = false;

            Invoke(() =>
            {
                using (var dialog = new FileConflictDialog(fileName, sourcePath, destPath))
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        action = dialog.Action;
                        applyToAll = dialog.ApplyToAll;
                    }
                }
            });

            // Resume stopwatch after dialog
            if (action != FileConflictAction.Cancel)
            {
                transferStopwatch.Start();
            }

            // If "Apply to all" was checked, remember the action
            if (applyToAll)
            {
                applyToAllAction = action;
            }

            return action;
        }

        private void UpdateTransferSpeed(long copiedBytes)
        {
            var now = DateTime.Now;
            var timeSinceLastUpdate = (now - lastSpeedUpdate).TotalSeconds;

            // Update speed calculation every 0.5 seconds
            if (timeSinceLastUpdate >= 0.5)
            {
                long bytesSinceLastUpdate = copiedBytes - lastBytesTransferred;
                double bytesPerSecond = bytesSinceLastUpdate / timeSinceLastUpdate;
                double bitsPerSecond = bytesPerSecond * 8;

                lastBytesTransferred = copiedBytes;
                lastSpeedUpdate = now;
                currentSpeed = FormatSpeed(bytesPerSecond, false);
                currentSpeedBits = FormatSpeed(bitsPerSecond, true);
            }
            // Keep showing the last calculated speed
        }

        private string FormatSpeed(double bytesPerSecond, bool isBits)
        {
            string[] sizes = isBits ? new[] { "b/s", "Kb/s", "Mb/s", "Gb/s" } : new[] { "B/s", "KB/s", "MB/s", "GB/s" };
            double speed = bytesPerSecond;
            int order = 0;
            while (speed >= 1024 && order < sizes.Length - 1)
            {
                order++;
                speed = speed / 1024;
            }
            return $"{speed:0.##} {sizes[order]}";
        }

        private async void StartFileCopy(string sourceFile, string destFolder, CancellationToken ct)
        {
            try
            {
                string fileName = Path.GetFileName(sourceFile);
                string destFile = Path.Combine(destFolder, fileName);

                // Check if file already exists
                if (File.Exists(destFile))
                {
                    var action = HandleFileConflict(fileName, sourceFile, destFile);

                    if (action == FileConflictAction.Cancel)
                    {
                        lblStatus.Text = "Cancelled by user";
                        btnCancel.Text = "Close";
                        return;
                    }
                    else if (action == FileConflictAction.Skip)
                    {
                        lblStatus.Text = "File skipped";
                        btnCancel.Text = "Close";
                        await Task.Delay(1000);
                        Close();
                        return;
                    }
                    // If Replace, continue with the copy (which will overwrite)
                }

                lblStatus.Text = $"Copying {fileName}...";
                progressBar.Style = ProgressBarStyle.Continuous;

                await Task.Run(() =>
                {
                    using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
                    using (var destStream = new FileStream(destFile, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[81920];
                        long totalBytes = sourceStream.Length;
                        long copiedBytes = 0;
                        int bytesRead;

                        while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                destStream.Close();
                                File.Delete(destFile);
                                Invoke(() => lblStatus.Text = "Cancelled");
                                return;
                            }

                            destStream.Write(buffer, 0, bytesRead);
                            copiedBytes += bytesRead;

                            int progress = (int)((copiedBytes * 100) / totalBytes);
                            UpdateTransferSpeed(copiedBytes);

                            Invoke(() =>
                            {
                                progressBar.Value = Math.Min(progress, 100);
                                string statusText = $"Copying... {FormatBytes(copiedBytes)} / {FormatBytes(totalBytes)} ({progress}%)";
                                if (!string.IsNullOrEmpty(currentSpeed))
                                {
                                    statusText += $"\n{currentSpeed} ({currentSpeedBits})";
                                }
                                lblStatus.Text = statusText;
                            });
                        }
                    }
                }, ct);

                if (!ct.IsCancellationRequested)
                {
                    transferStopwatch.Stop();

                    if (isMoveOperation)
                    {
                        lblStatus.Text = "Deleting source file...";
                        File.Delete(sourceFile);
                    }

                    var elapsed = transferStopwatch.Elapsed;
                    lblStatus.Text = $"{(isMoveOperation ? "Move" : "Copy")} completed successfully! (Time: {elapsed.TotalSeconds:0.0}s)";
                    btnCancel.Text = "Close";

                    await Task.Delay(2000);
                    if (!ct.IsCancellationRequested)
                        Close();
                }
            }
            catch (Exception ex)
            {
                transferStopwatch.Stop();
                MessageBox.Show($"Error: {ex.Message}", "Transfer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"Error: {ex.Message}";
                btnCancel.Text = "Close";
            }
        }

        private async void StartFolderCopy(string sourceFolder, string destFolder, CancellationToken ct)
        {
            try
            {
                string folderName = Path.GetFileName(sourceFolder.TrimEnd(Path.DirectorySeparatorChar));
                string finalDest = Path.Combine(destFolder, folderName);

                // Check if destination folder already exists
                if (Directory.Exists(finalDest))
                {
                    // Pause stopwatch during dialog
                    transferStopwatch.Stop();

                    var result = MessageBox.Show(
                        $"The folder '{folderName}' already exists in the destination.\n\n" +
                        "Do you want to merge the contents?\n\n" +
                        "Yes = Merge (replace conflicting files)\n" +
                        "No = Skip this operation\n" +
                        "Cancel = Cancel transfer",
                        "Folder Already Exists",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Cancel)
                    {
                        lblStatus.Text = "Cancelled by user";
                        btnCancel.Text = "Close";
                        return;
                    }
                    else if (result == DialogResult.No)
                    {
                        lblStatus.Text = "Folder skipped";
                        btnCancel.Text = "Close";
                        await Task.Delay(1000);
                        Close();
                        return;
                    }

                    // Resume stopwatch if continuing
                    transferStopwatch.Start();
                }

                lblStatus.Text = $"{(isMoveOperation ? "Moving" : "Copying")} folder...";
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = 0;

                // Start calculating folder size in parallel
                Task.Run(() => CalculateFolderSize(sourceFolder, ct), ct);

                await Task.Run(() =>
                {
                    // Add /BYTES /NP to get parseable output
                    string robocopyArgs = isMoveOperation
                        ? $"\"{sourceFolder}\" \"{finalDest}\" /E /MOVE /MT:8 /R:0 /W:0 /BYTES /NP"
                        : $"\"{sourceFolder}\" \"{finalDest}\" /E /MT:8 /R:0 /W:0 /BYTES /NP";

                    robocopyProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "robocopy",
                            Arguments = robocopyArgs,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };

                    robocopyProcess.OutputDataReceived += (s, e) =>
                    {
                        if (e.Data != null && !string.IsNullOrWhiteSpace(e.Data))
                        {
                            try
                            {
                                ParseRobocopyOutput(e.Data);
                            }
                            catch { }
                        }
                    };

                    robocopyProcess.Start();
                    robocopyProcess.BeginOutputReadLine();
                    robocopyProcess.WaitForExit();

                    if (ct.IsCancellationRequested)
                        return;

                    int exitCode = robocopyProcess.ExitCode;

                    // Robocopy exit codes: 0-7 are success, 8+ are errors
                    if (exitCode < 8)
                    {
                        transferStopwatch.Stop();
                        var elapsed = transferStopwatch.Elapsed;

                        Invoke(() =>
                        {
                            progressBar.Style = ProgressBarStyle.Continuous;
                            progressBar.Value = 100;
                            lblStatus.Text = $"{(isMoveOperation ? "Move" : "Copy")} completed successfully! (Time: {elapsed.TotalSeconds:0.0}s)";
                            btnCancel.Text = "Close";
                        });

                        Thread.Sleep(2000);
                        if (!ct.IsCancellationRequested)
                            Invoke(Close);
                    }
                    else
                    {
                        transferStopwatch.Stop();
                        Invoke(() =>
                        {
                            lblStatus.Text = $"Error: Robocopy exit code {exitCode}";
                            btnCancel.Text = "Close";
                        });
                    }
                }, ct);
            }
            catch (Exception ex)
            {
                transferStopwatch.Stop();
                MessageBox.Show($"Error: {ex.Message}", "Transfer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = $"Error: {ex.Message}";
                btnCancel.Text = "Close";
            }
        }

        private void ParseRobocopyOutput(string line)
        {
            try
            {
                // Robocopy with /BYTES shows progress like: "100%    New File   12345   filename.ext"
                // or shows current file being copied

                // Try to match percentage pattern
                var percentMatch = Regex.Match(line, @"(\d+)%");
                if (percentMatch.Success)
                {
                    int filePercent = int.Parse(percentMatch.Groups[1].Value);

                    // Try to extract file size from the line
                    var sizeMatch = Regex.Match(line, @"\s+(\d+)\s+");
                    if (sizeMatch.Success && long.TryParse(sizeMatch.Groups[1].Value, out long fileSize))
                    {
                        // Estimate bytes transferred for this file
                        long fileBytesTransferred = (fileSize * filePercent) / 100;

                        // Update total if we haven't counted this file's previous progress
                        if (filePercent == 100)
                        {
                            filesTransferred++;
                            totalBytesTransferred += fileSize;
                        }
                    }
                }

                // Match summary line like "Bytes : 123456 100% 789012"
                var summaryMatch = Regex.Match(line, @"Bytes\s*:\s*(\d+)");
                if (summaryMatch.Success && long.TryParse(summaryMatch.Groups[1].Value, out long totalBytes))
                {
                    totalBytesTransferred = totalBytes;
                }

                // Update UI with current progress
                UpdateFolderProgress();
            }
            catch { }
        }

        private void UpdateFolderProgress()
        {
            try
            {
                Invoke(() =>
                {
                    int progress = 0;

                    if (totalFolderSize > 0 && totalBytesTransferred > 0)
                    {
                        progress = (int)((totalBytesTransferred * 100) / totalFolderSize);
                        progress = Math.Min(progress, 100);
                        progressBar.Value = progress;
                    }
                    else if (totalBytesTransferred > 0)
                    {
                        // Show marquee if we don't know total size yet
                        if (progressBar.Style != ProgressBarStyle.Marquee)
                        {
                            progressBar.Style = ProgressBarStyle.Marquee;
                        }
                    }

                    // Update speed calculation
                    var now = DateTime.Now;
                    var timeSinceLastUpdate = (now - lastSpeedUpdate).TotalSeconds;

                    if (timeSinceLastUpdate >= 0.5)
                    {
                        long bytesSinceLastUpdate = totalBytesTransferred - lastBytesTransferred;
                        double bytesPerSecond = bytesSinceLastUpdate / timeSinceLastUpdate;
                        double bitsPerSecond = bytesPerSecond * 8;

                        lastBytesTransferred = totalBytesTransferred;
                        lastSpeedUpdate = now;
                        currentSpeed = FormatSpeed(bytesPerSecond, false);
                        currentSpeedBits = FormatSpeed(bitsPerSecond, true);
                    }

                    // Build status text
                    string statusText = isMoveOperation ? "Moving folder..." : "Copying folder...";

                    if (totalFolderSize > 0 && totalBytesTransferred > 0)
                    {
                        statusText = $"{statusText}\n{FormatBytes(totalBytesTransferred)} / {FormatBytes(totalFolderSize)} ({progress}%)";
                    }
                    else if (totalBytesTransferred > 0)
                    {
                        statusText = $"{statusText}\n{FormatBytes(totalBytesTransferred)} transferred";
                        if (totalFolderSize > 0)
                        {
                            // We know the size now, switch to continuous progress bar
                            if (progressBar.Style != ProgressBarStyle.Continuous)
                            {
                                progressBar.Style = ProgressBarStyle.Continuous;
                                progressBar.Value = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(currentSpeed))
                    {
                        statusText += $"\n{currentSpeed} ({currentSpeedBits})";
                    }

                    if (totalFiles > 0 && filesTransferred > 0)
                    {
                        statusText += $"\n{filesTransferred} / {totalFiles} files";
                    }
                    else if (filesTransferred > 0)
                    {
                        statusText += $"\n{filesTransferred} files transferred";
                    }

                    lblStatus.Text = statusText;
                });
            }
            catch { }
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (btnCancel.Text == "Close")
            {
                Close();
            }
            else
            {
                transferStopwatch.Stop();
                cancellationTokenSource?.Cancel();
                robocopyProcess?.Kill();
                lblStatus.Text = "Cancelling...";
                btnCancel.Enabled = false;

                Task.Run(async () =>
                {
                    await Task.Delay(500);
                    Invoke(Close);
                });
            }
        }

        private void TransferProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            transferStopwatch?.Stop();
            cancellationTokenSource?.Cancel();
            robocopyProcess?.Kill();
        }

        private void TransferProgressForm_Load(object sender, EventArgs e)
        {

        }

        private void lblOperationLabel_Click(object sender, EventArgs e)
        {

        }

        private void lblDest_Click(object sender, EventArgs e)
        {

        }
    }
}
