namespace Archiver
{
    partial class ArchiveProgressForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbrProgress = new System.Windows.Forms.ProgressBar();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.gbxProgress = new System.Windows.Forms.GroupBox();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.gbxEvents = new System.Windows.Forms.GroupBox();
            this.tbxEvents = new System.Windows.Forms.TextBox();
            this.gbxProgress.SuspendLayout();
            this.gbxEvents.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbrProgress
            // 
            this.pbrProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbrProgress.Location = new System.Drawing.Point(6, 19);
            this.pbrProgress.MarqueeAnimationSpeed = 1;
            this.pbrProgress.Name = "pbrProgress";
            this.pbrProgress.Size = new System.Drawing.Size(553, 23);
            this.pbrProgress.TabIndex = 0;
            // 
            // btnStartStop
            // 
            this.btnStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartStop.Location = new System.Drawing.Point(502, 275);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(75, 23);
            this.btnStartStop.TabIndex = 1;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = true;
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(15, 66);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 26);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Files: -\r\nSize: -";
            // 
            // gbxProgress
            // 
            this.gbxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxProgress.Controls.Add(this.lblCurrentFile);
            this.gbxProgress.Controls.Add(this.lblTime);
            this.gbxProgress.Controls.Add(this.pbrProgress);
            this.gbxProgress.Controls.Add(this.lblStatus);
            this.gbxProgress.Enabled = false;
            this.gbxProgress.Location = new System.Drawing.Point(12, 12);
            this.gbxProgress.Name = "gbxProgress";
            this.gbxProgress.Size = new System.Drawing.Size(565, 101);
            this.gbxProgress.TabIndex = 3;
            this.gbxProgress.TabStop = false;
            this.gbxProgress.Text = "Progress";
            // 
            // lblCurrentFile
            // 
            this.lblCurrentFile.AutoSize = true;
            this.lblCurrentFile.Location = new System.Drawing.Point(6, 45);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new System.Drawing.Size(69, 13);
            this.lblCurrentFile.TabIndex = 4;
            this.lblCurrentFile.Text = "Current File: -";
            // 
            // lblTime
            // 
            this.lblTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTime.AutoSize = true;
            this.lblTime.BackColor = System.Drawing.Color.Transparent;
            this.lblTime.Location = new System.Drawing.Point(426, 66);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(39, 26);
            this.lblTime.TabIndex = 3;
            this.lblTime.Text = "Time: -\r\nETA: -";
            // 
            // gbxEvents
            // 
            this.gbxEvents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxEvents.Controls.Add(this.tbxEvents);
            this.gbxEvents.Enabled = false;
            this.gbxEvents.Location = new System.Drawing.Point(12, 119);
            this.gbxEvents.Name = "gbxEvents";
            this.gbxEvents.Size = new System.Drawing.Size(565, 150);
            this.gbxEvents.TabIndex = 4;
            this.gbxEvents.TabStop = false;
            this.gbxEvents.Text = "Events";
            // 
            // tbxEvents
            // 
            this.tbxEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxEvents.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxEvents.Location = new System.Drawing.Point(3, 16);
            this.tbxEvents.Multiline = true;
            this.tbxEvents.Name = "tbxEvents";
            this.tbxEvents.ReadOnly = true;
            this.tbxEvents.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxEvents.Size = new System.Drawing.Size(559, 131);
            this.tbxEvents.TabIndex = 0;
            this.tbxEvents.WordWrap = false;
            // 
            // ArchiveProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 310);
            this.Controls.Add(this.gbxEvents);
            this.Controls.Add(this.gbxProgress);
            this.Controls.Add(this.btnStartStop);
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "ArchiveProgressForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Archive -";
            this.gbxProgress.ResumeLayout(false);
            this.gbxProgress.PerformLayout();
            this.gbxEvents.ResumeLayout(false);
            this.gbxEvents.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbrProgress;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox gbxProgress;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.GroupBox gbxEvents;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.TextBox tbxEvents;
    }
}

