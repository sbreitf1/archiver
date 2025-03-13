namespace Archiver
{
    partial class MainForm
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
            this.lvwSets = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnDeleteSet = new System.Windows.Forms.Button();
            this.btnExecuteSet = new System.Windows.Forms.Button();
            this.btnEditSet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvwSets
            // 
            this.lvwSets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwSets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvwSets.FullRowSelect = true;
            this.lvwSets.GridLines = true;
            this.lvwSets.HideSelection = false;
            this.lvwSets.Location = new System.Drawing.Point(12, 12);
            this.lvwSets.Name = "lvwSets";
            this.lvwSets.Size = new System.Drawing.Size(633, 241);
            this.lvwSets.TabIndex = 0;
            this.lvwSets.UseCompatibleStateImageBehavior = false;
            this.lvwSets.View = System.Windows.Forms.View.Details;
            this.lvwSets.SelectedIndexChanged += new System.EventHandler(this.lvwSets_SelectedIndexChanged);
            this.lvwSets.DoubleClick += new System.EventHandler(this.lvwSets_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Backup Dir";
            this.columnHeader2.Width = 300;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Destination";
            this.columnHeader3.Width = 120;
            // 
            // btnAddNew
            // 
            this.btnAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNew.Location = new System.Drawing.Point(651, 12);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(137, 23);
            this.btnAddNew.TabIndex = 1;
            this.btnAddNew.Text = "Add new Archive Set";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // btnDeleteSet
            // 
            this.btnDeleteSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteSet.Enabled = false;
            this.btnDeleteSet.Location = new System.Drawing.Point(651, 70);
            this.btnDeleteSet.Name = "btnDeleteSet";
            this.btnDeleteSet.Size = new System.Drawing.Size(137, 23);
            this.btnDeleteSet.TabIndex = 2;
            this.btnDeleteSet.Text = "Delete Archive Set(s)";
            this.btnDeleteSet.UseVisualStyleBackColor = true;
            this.btnDeleteSet.Click += new System.EventHandler(this.btnDeleteSet_Click);
            // 
            // btnExecuteSet
            // 
            this.btnExecuteSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecuteSet.Enabled = false;
            this.btnExecuteSet.Location = new System.Drawing.Point(651, 230);
            this.btnExecuteSet.Name = "btnExecuteSet";
            this.btnExecuteSet.Size = new System.Drawing.Size(137, 23);
            this.btnExecuteSet.TabIndex = 3;
            this.btnExecuteSet.Text = "Execute";
            this.btnExecuteSet.UseVisualStyleBackColor = true;
            this.btnExecuteSet.Click += new System.EventHandler(this.btnExecuteSet_Click);
            // 
            // btnEditSet
            // 
            this.btnEditSet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditSet.Enabled = false;
            this.btnEditSet.Location = new System.Drawing.Point(651, 41);
            this.btnEditSet.Name = "btnEditSet";
            this.btnEditSet.Size = new System.Drawing.Size(137, 23);
            this.btnEditSet.TabIndex = 4;
            this.btnEditSet.Text = "Edit Archive Set";
            this.btnEditSet.UseVisualStyleBackColor = true;
            this.btnEditSet.Click += new System.EventHandler(this.btnEditSet_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 265);
            this.Controls.Add(this.btnEditSet);
            this.Controls.Add(this.btnExecuteSet);
            this.Controls.Add(this.btnDeleteSet);
            this.Controls.Add(this.btnAddNew);
            this.Controls.Add(this.lvwSets);
            this.MinimumSize = new System.Drawing.Size(500, 250);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Archiver";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvwSets;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnDeleteSet;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button btnExecuteSet;
        private System.Windows.Forms.Button btnEditSet;
    }
}