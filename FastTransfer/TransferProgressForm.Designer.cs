namespace FastTransfer
{
    partial class TransferProgressForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblSourceLabel;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.Label lblDestLabel;
        private System.Windows.Forms.Label lblDest;
        private System.Windows.Forms.Label lblOperationLabel;
        private System.Windows.Forms.Label lblOperation;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblSourceLabel = new Label();
            lblSource = new Label();
            lblDestLabel = new Label();
            lblDest = new Label();
            lblOperationLabel = new Label();
            lblOperation = new Label();
            progressBar = new ProgressBar();
            lblStatus = new Label();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblSourceLabel
            // 
            lblSourceLabel.AutoSize = true;
            lblSourceLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSourceLabel.Location = new Point(12, 13);
            lblSourceLabel.Name = "lblSourceLabel";
            lblSourceLabel.Size = new Size(86, 30);
            lblSourceLabel.TabIndex = 0;
            lblSourceLabel.Text = "Source:";
            // 
            // lblSource
            // 
            lblSource.Location = new Point(12, 35);
            lblSource.Name = "lblSource";
            lblSource.Size = new Size(560, 40);
            lblSource.TabIndex = 1;
            // 
            // lblDestLabel
            // 
            lblDestLabel.AutoSize = true;
            lblDestLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDestLabel.Location = new Point(12, 78);
            lblDestLabel.Name = "lblDestLabel";
            lblDestLabel.Size = new Size(132, 30);
            lblDestLabel.TabIndex = 2;
            lblDestLabel.Text = "Destination:";
            // 
            // lblDest
            // 
            lblDest.Location = new Point(12, 100);
            lblDest.Name = "lblDest";
            lblDest.Size = new Size(560, 40);
            lblDest.TabIndex = 3;
            lblDest.Click += lblDest_Click;
            // 
            // lblOperationLabel
            // 
            lblOperationLabel.AutoSize = true;
            lblOperationLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblOperationLabel.Location = new Point(12, 140);
            lblOperationLabel.Name = "lblOperationLabel";
            lblOperationLabel.Size = new Size(118, 30);
            lblOperationLabel.TabIndex = 4;
            lblOperationLabel.Text = "Operation:";
            lblOperationLabel.Click += lblOperationLabel_Click;
            // 
            // lblOperation
            // 
            lblOperation.AutoSize = true;
            lblOperation.Location = new Point(130, 140);
            lblOperation.Name = "lblOperation";
            lblOperation.Size = new Size(0, 30);
            lblOperation.TabIndex = 5;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 175);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(560, 30);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 6;
            // 
            // lblStatus
            // 
            lblStatus.Location = new Point(12, 210);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(560, 92);
            lblStatus.TabIndex = 7;
            lblStatus.Text = "Starting...";
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(482, 305);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(90, 40);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // TransferProgressForm
            // 
            ClientSize = new Size(590, 357);
            Controls.Add(lblSourceLabel);
            Controls.Add(lblSource);
            Controls.Add(lblDestLabel);
            Controls.Add(lblDest);
            Controls.Add(lblOperationLabel);
            Controls.Add(lblOperation);
            Controls.Add(progressBar);
            Controls.Add(lblStatus);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "TransferProgressForm";
            Text = "Transfer Progress";
            FormClosing += TransferProgressForm_FormClosing;
            Load += TransferProgressForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
