using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archiver
{
    public partial class EditArchiveSetDialog: Form
    {
        public ArchiveSet ArchiveSet { get; set; }

        public EditArchiveSetDialog()
        {
            InitializeComponent();
        }

        private void EditArchiveSetDialog_Shown(object sender, EventArgs e)
        {
            tbxName.Text = this.ArchiveSet.Name;
            lblLocalDir.Text = (string.IsNullOrWhiteSpace(this.ArchiveSet.BackupDir) ? "-" : this.ArchiveSet.BackupDir);
            lblDestinationDir.Text = (string.IsNullOrWhiteSpace(this.ArchiveSet.DestinationDir) ? "-" : this.ArchiveSet.DestinationDir);

            tbxName.SelectAll();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.ArchiveSet.Name = tbxName.Text;
            if (string.IsNullOrWhiteSpace(this.ArchiveSet.BackupDir))
            {
                MessageBox.Show(this, "Select a Local Directory first.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(this.ArchiveSet.DestinationDir))
            {
                MessageBox.Show(this, "Select a Destination Directory first.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!Directory.Exists(this.ArchiveSet.BackupDir))
            {
                MessageBox.Show(this, "The selected Local Directory does not exist.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                Directory.CreateDirectory(this.ArchiveSet.DestinationDir);
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, "Unable to access Destination Directory: " + ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSelectLocalDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = (string.IsNullOrWhiteSpace(this.ArchiveSet.BackupDir) ? null : this.ArchiveSet.BackupDir);
            dialog.ShowNewFolderButton = false;
            dialog.Description = "Select a local folder to be archived:";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.ArchiveSet.BackupDir = dialog.SelectedPath;
                lblLocalDir.Text = this.ArchiveSet.BackupDir;
            }
        }

        private void btnSelectDestinationDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = (string.IsNullOrWhiteSpace(this.ArchiveSet.DestinationDir) ? null : this.ArchiveSet.DestinationDir);
            dialog.ShowNewFolderButton = true;
            dialog.Description = "Select the destination folder to write the archive to:";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.ArchiveSet.DestinationDir = dialog.SelectedPath;
                lblDestinationDir.Text = this.ArchiveSet.DestinationDir;
            }
        }
    }
}
