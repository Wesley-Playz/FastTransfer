using SHDocVw;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FastTransfer
{
    public partial class MainForm : Form
    {
        private Process robocopyProcess;
        private CancellationTokenSource cancellationTokenSource;
        private bool isFile;
        private bool isOperationInProgress;

        public MainForm(string sourcePath)
        {
            InitializeComponent();
            txtSource.Text = sourcePath;
            isFile = File.Exists(sourcePath);
            isOperationInProgress = false;
            LoadExplorerWindows();
        }

        private void LoadExplorerWindows()
        {
            var windows = Shell32Helper.GetOpenExplorerPaths();

            // Filter out the source directory
            string sourceDir = isFile ? Path.GetDirectoryName(txtSource.Text) : txtSource.Text;
            foreach (var window in windows)
            {
                if (!string.Equals(window, sourceDir, StringComparison.OrdinalIgnoreCase))
                {
                    cmbDestinations.Items.Add(window);
                }
            }

            if (cmbDestinations.Items.Count > 0)
                cmbDestinations.SelectedIndex = 0;
            else
            {
                MessageBox.Show("No other Explorer windows found. Please open a destination folder in Explorer.", "No Destinations", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnStart.Enabled = false;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cmbDestinations.SelectedItem == null)
            {
                MessageBox.Show("Please select a destination folder.", "No Destination", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isOperationInProgress)
            {
                MessageBox.Show("An operation is already in progress.", "Operation In Progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            isOperationInProgress = true;
            btnStart.Enabled = false;
            btnCancel.Enabled = true;
            cmbDestinations.Enabled = false;
            rbCopy.Enabled = false;
            rbMove.Enabled = false;
            progressBar.Value = 0;
            progressBar.Style = ProgressBarStyle.Continuous;

            string source = txtSource.Text;
            string dest = cmbDestinations.SelectedItem.ToString();
            bool isMoveOperation = rbMove.Checked;

            cancellationTokenSource = new CancellationTokenSource();

            // Create a new window for this transfer to allow multiple transfers
            var transferWindow = new TransferProgressForm(source, dest, isFile, isMoveOperation);
            transferWindow.Show();

            // Reset this form to allow another transfer
            Task.Run(async () =>
            {
                await Task.Delay(500); // Small delay to ensure new window is visible
                Invoke(() =>
                {
                    isOperationInProgress = false;
                    btnStart.Enabled = true;
                    btnCancel.Enabled = false;
                    cmbDestinations.Enabled = true;
                    rbCopy.Enabled = true;
                    rbMove.Enabled = true;
                    progressBar.Value = 0;
                    lblStatus.Text = "Ready for next transfer";
                });
            });
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            cancellationTokenSource?.Cancel();
            robocopyProcess?.Kill();
            base.OnFormClosing(e);
        }

        private void rbMove_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
