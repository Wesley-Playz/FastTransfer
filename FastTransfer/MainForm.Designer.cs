namespace FastTransfer
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.ComboBox cmbDestinations;
        private System.Windows.Forms.Label lblOperation;
        private System.Windows.Forms.RadioButton rbCopy;
        private System.Windows.Forms.RadioButton rbMove;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblSource = new Label();
            txtSource = new TextBox();
            lblDestination = new Label();
            cmbDestinations = new ComboBox();
            lblOperation = new Label();
            rbCopy = new RadioButton();
            rbMove = new RadioButton();
            progressBar = new ProgressBar();
            lblStatus = new Label();
            btnStart = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblSource
            // 
            lblSource.AutoSize = true;
            lblSource.Location = new Point(12, 15);
            lblSource.Name = "lblSource";
            lblSource.Size = new Size(81, 30);
            lblSource.TabIndex = 0;
            lblSource.Text = "Source:";
            // 
            // txtSource
            // 
            txtSource.Location = new Point(100, 12);
            txtSource.Name = "txtSource";
            txtSource.ReadOnly = true;
            txtSource.Size = new Size(563, 35);
            txtSource.TabIndex = 1;
            // 
            // lblDestination
            // 
            lblDestination.AutoSize = true;
            lblDestination.Location = new Point(12, 50);
            lblDestination.Name = "lblDestination";
            lblDestination.Size = new Size(124, 30);
            lblDestination.TabIndex = 2;
            lblDestination.Text = "Destination:";
            // 
            // cmbDestinations
            // 
            cmbDestinations.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDestinations.Location = new Point(148, 47);
            cmbDestinations.Name = "cmbDestinations";
            cmbDestinations.Size = new Size(515, 38);
            cmbDestinations.TabIndex = 3;
            // 
            // lblOperation
            // 
            lblOperation.AutoSize = true;
            lblOperation.Location = new Point(12, 85);
            lblOperation.Name = "lblOperation";
            lblOperation.Size = new Size(111, 30);
            lblOperation.TabIndex = 4;
            lblOperation.Text = "Operation:";
            // 
            // rbCopy
            // 
            rbCopy.AutoSize = true;
            rbCopy.Checked = true;
            rbCopy.Location = new Point(132, 83);
            rbCopy.Name = "rbCopy";
            rbCopy.Size = new Size(85, 34);
            rbCopy.TabIndex = 5;
            rbCopy.TabStop = true;
            rbCopy.Text = "Copy";
            // 
            // rbMove
            // 
            rbMove.AutoSize = true;
            rbMove.Location = new Point(234, 84);
            rbMove.Name = "rbMove";
            rbMove.Size = new Size(90, 34);
            rbMove.TabIndex = 6;
            rbMove.Text = "Move";
            rbMove.CheckedChanged += rbMove_CheckedChanged;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(15, 119);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(648, 25);
            progressBar.TabIndex = 7;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(15, 145);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(69, 30);
            lblStatus.TabIndex = 8;
            lblStatus.Text = "Ready";
            // 
            // btnStart
            // 
            btnStart.Location = new Point(375, 161);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(90, 44);
            btnStart.TabIndex = 9;
            btnStart.Text = "Start";
            btnStart.Click += btnStart_Click;
            // 
            // btnCancel
            // 
            btnCancel.Enabled = false;
            btnCancel.Location = new Point(480, 161);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 44);
            btnCancel.TabIndex = 10;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // MainForm
            // 
            ClientSize = new Size(675, 215);
            Controls.Add(lblSource);
            Controls.Add(txtSource);
            Controls.Add(lblDestination);
            Controls.Add(cmbDestinations);
            Controls.Add(lblOperation);
            Controls.Add(rbCopy);
            Controls.Add(rbMove);
            Controls.Add(progressBar);
            Controls.Add(lblStatus);
            Controls.Add(btnStart);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "FastTransfer";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
