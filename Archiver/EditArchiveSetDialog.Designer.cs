namespace Archiver
{
    partial class EditArchiveSetDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbxName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblLocalDirLabel = new System.Windows.Forms.Label();
            this.lblDestinationDirLabel = new System.Windows.Forms.Label();
            this.lblLocalDir = new System.Windows.Forms.Label();
            this.lblDestinationDir = new System.Windows.Forms.Label();
            this.btnSelectLocalDir = new System.Windows.Forms.Button();
            this.btnSelectDestinationDir = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbxName
            // 
            this.tbxName.Location = new System.Drawing.Point(99, 12);
            this.tbxName.Name = "tbxName";
            this.tbxName.Size = new System.Drawing.Size(255, 20);
            this.tbxName.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 15);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name";
            // 
            // lblLocalDirLabel
            // 
            this.lblLocalDirLabel.AutoSize = true;
            this.lblLocalDirLabel.Location = new System.Drawing.Point(12, 41);
            this.lblLocalDirLabel.Name = "lblLocalDirLabel";
            this.lblLocalDirLabel.Size = new System.Drawing.Size(52, 13);
            this.lblLocalDirLabel.TabIndex = 2;
            this.lblLocalDirLabel.Text = "Local Dir:";
            // 
            // lblDestinationDirLabel
            // 
            this.lblDestinationDirLabel.AutoSize = true;
            this.lblDestinationDirLabel.Location = new System.Drawing.Point(12, 70);
            this.lblDestinationDirLabel.Name = "lblDestinationDirLabel";
            this.lblDestinationDirLabel.Size = new System.Drawing.Size(79, 13);
            this.lblDestinationDirLabel.TabIndex = 6;
            this.lblDestinationDirLabel.Text = "Destination Dir:";
            // 
            // lblLocalDir
            // 
            this.lblLocalDir.AutoSize = true;
            this.lblLocalDir.Location = new System.Drawing.Point(180, 41);
            this.lblLocalDir.Name = "lblLocalDir";
            this.lblLocalDir.Size = new System.Drawing.Size(10, 13);
            this.lblLocalDir.TabIndex = 7;
            this.lblLocalDir.Text = "-";
            // 
            // lblDestinationDir
            // 
            this.lblDestinationDir.AutoSize = true;
            this.lblDestinationDir.Location = new System.Drawing.Point(180, 70);
            this.lblDestinationDir.Name = "lblDestinationDir";
            this.lblDestinationDir.Size = new System.Drawing.Size(10, 13);
            this.lblDestinationDir.TabIndex = 8;
            this.lblDestinationDir.Text = "-";
            // 
            // btnSelectLocalDir
            // 
            this.btnSelectLocalDir.Location = new System.Drawing.Point(99, 36);
            this.btnSelectLocalDir.Name = "btnSelectLocalDir";
            this.btnSelectLocalDir.Size = new System.Drawing.Size(75, 23);
            this.btnSelectLocalDir.TabIndex = 9;
            this.btnSelectLocalDir.Text = "Select";
            this.btnSelectLocalDir.UseVisualStyleBackColor = true;
            this.btnSelectLocalDir.Click += new System.EventHandler(this.btnSelectLocalDir_Click);
            // 
            // btnSelectDestinationDir
            // 
            this.btnSelectDestinationDir.Location = new System.Drawing.Point(99, 65);
            this.btnSelectDestinationDir.Name = "btnSelectDestinationDir";
            this.btnSelectDestinationDir.Size = new System.Drawing.Size(75, 23);
            this.btnSelectDestinationDir.TabIndex = 10;
            this.btnSelectDestinationDir.Text = "Select";
            this.btnSelectDestinationDir.UseVisualStyleBackColor = true;
            this.btnSelectDestinationDir.Click += new System.EventHandler(this.btnSelectDestinationDir_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(404, 125);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(323, 125);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // EditArchiveSetDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(491, 160);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnSelectDestinationDir);
            this.Controls.Add(this.btnSelectLocalDir);
            this.Controls.Add(this.lblDestinationDir);
            this.Controls.Add(this.lblLocalDir);
            this.Controls.Add(this.lblDestinationDirLabel);
            this.Controls.Add(this.lblLocalDirLabel);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.tbxName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditArchiveSetDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Archive Set";
            this.Shown += new System.EventHandler(this.EditArchiveSetDialog_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblLocalDirLabel;
        private System.Windows.Forms.Label lblDestinationDirLabel;
        private System.Windows.Forms.Label lblLocalDir;
        private System.Windows.Forms.Label lblDestinationDir;
        private System.Windows.Forms.Button btnSelectLocalDir;
        private System.Windows.Forms.Button btnSelectDestinationDir;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}