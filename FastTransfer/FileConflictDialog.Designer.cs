namespace FastTransfer
{
    partial class FileConflictDialog
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblSourceLabel;
        private System.Windows.Forms.Label lblSourceInfo;
        private System.Windows.Forms.Label lblDestLabel;
        private System.Windows.Forms.Label lblDestInfo;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnSkip;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkApplyToAll;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblMessage = new Label();
            lblSourceLabel = new Label();
            lblSourceInfo = new Label();
            lblDestLabel = new Label();
            lblDestInfo = new Label();
            btnReplace = new Button();
            btnSkip = new Button();
            btnCancel = new Button();
            chkApplyToAll = new CheckBox();
            SuspendLayout();
            // 
            // lblMessage
            // 
            lblMessage.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblMessage.Location = new Point(12, 12);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(749, 86);
            lblMessage.TabIndex = 0;
            // 
            // lblSourceLabel
            // 
            lblSourceLabel.AutoSize = true;
            lblSourceLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSourceLabel.Location = new Point(12, 98);
            lblSourceLabel.Name = "lblSourceLabel";
            lblSourceLabel.Size = new Size(123, 30);
            lblSourceLabel.TabIndex = 1;
            lblSourceLabel.Text = "Source file:";
            // 
            // lblSourceInfo
            // 
            lblSourceInfo.Location = new Point(12, 137);
            lblSourceInfo.Name = "lblSourceInfo";
            lblSourceInfo.Size = new Size(460, 35);
            lblSourceInfo.TabIndex = 2;
            // 
            // lblDestLabel
            // 
            lblDestLabel.AutoSize = true;
            lblDestLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDestLabel.Location = new Point(12, 198);
            lblDestLabel.Name = "lblDestLabel";
            lblDestLabel.Size = new Size(134, 30);
            lblDestLabel.TabIndex = 3;
            lblDestLabel.Text = "Existing file:";
            // 
            // lblDestInfo
            // 
            lblDestInfo.Location = new Point(12, 238);
            lblDestInfo.Name = "lblDestInfo";
            lblDestInfo.Size = new Size(460, 35);
            lblDestInfo.TabIndex = 4;
            // 
            // btnReplace
            // 
            btnReplace.Location = new Point(377, 314);
            btnReplace.Name = "btnReplace";
            btnReplace.Size = new Size(124, 49);
            btnReplace.TabIndex = 6;
            btnReplace.Text = "Replace";
            btnReplace.Click += btnReplace_Click;
            // 
            // btnSkip
            // 
            btnSkip.Location = new Point(507, 314);
            btnSkip.Name = "btnSkip";
            btnSkip.Size = new Size(124, 49);
            btnSkip.TabIndex = 7;
            btnSkip.Text = "Skip";
            btnSkip.Click += btnSkip_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(637, 314);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(124, 49);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // chkApplyToAll
            // 
            chkApplyToAll.AutoSize = true;
            chkApplyToAll.Location = new Point(12, 329);
            chkApplyToAll.Name = "chkApplyToAll";
            chkApplyToAll.Size = new Size(227, 34);
            chkApplyToAll.TabIndex = 5;
            chkApplyToAll.Text = "Apply to all conflicts";
            // 
            // FileConflictDialog
            // 
            ClientSize = new Size(773, 375);
            Controls.Add(lblMessage);
            Controls.Add(lblSourceLabel);
            Controls.Add(lblSourceInfo);
            Controls.Add(lblDestLabel);
            Controls.Add(lblDestInfo);
            Controls.Add(chkApplyToAll);
            Controls.Add(btnReplace);
            Controls.Add(btnSkip);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FileConflictDialog";
            StartPosition = FormStartPosition.CenterParent;
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            Action = FileConflictAction.Replace;
            ApplyToAll = chkApplyToAll.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            Action = FileConflictAction.Skip;
            ApplyToAll = chkApplyToAll.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Action = FileConflictAction.Cancel;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
