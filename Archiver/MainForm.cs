using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Archiver
{
    public partial class MainForm : Form
    {
        List<ArchiveSet> sets;

        public MainForm()
        {
            InitializeComponent();

            LoadArchiveSets();

            ShowSets();
        }

        private void ShowSets()
        {
            this.lvwSets.Items.Clear();
            foreach (ArchiveSet set in this.sets)
            {
                ListViewItem item = new ListViewItem(set.Name);
                item.SubItems.Add(set.BackupDir);
                item.SubItems.Add(set.DestinationDir);
                this.lvwSets.Items.Add(item);
            }
        }

        private void lvwSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEditSet.Enabled = (lvwSets.SelectedItems.Count == 1);
            btnDeleteSet.Enabled = (lvwSets.SelectedItems.Count > 0);
            btnExecuteSet.Enabled = (lvwSets.SelectedItems.Count == 1);
        }

        private void lvwSets_DoubleClick(object sender, EventArgs e)
        {
            btnExecuteSet.PerformClick();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            EditArchiveSetDialog dialog = new EditArchiveSetDialog();
            dialog.ArchiveSet = new ArchiveSet();
            dialog.Owner = this;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.sets.Add(dialog.ArchiveSet);
                SaveArchiveSets();
                ShowSets();
                lvwSets.SelectedIndices.Clear();
                lvwSets.SelectedIndices.Add(this.sets.Count - 1);
            }
        }

        private void btnEditSet_Click(object sender, EventArgs e)
        {
            int index = lvwSets.SelectedIndices[0];
            EditArchiveSetDialog dialog = new EditArchiveSetDialog();
            dialog.ArchiveSet = this.sets[index];
            dialog.Owner = this;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.sets.RemoveAt(index);
                this.sets.Insert(index, dialog.ArchiveSet);
                SaveArchiveSets();
                ShowSets();
                lvwSets.SelectedIndices.Clear();
                lvwSets.SelectedIndices.Add(index);
            }
        }

        private void btnDeleteSet_Click(object sender, EventArgs e)
        {
            if (lvwSets.SelectedIndices.Count != 1)
            {
                MessageBox.Show(this, "Select only one Archive Set.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(this, "Delete selected Archive Set(s)? No data will be deleted, the Archive Sets will only disappear from this list.", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                //TODO delete multiple selected sets
                this.sets.RemoveAt(lvwSets.SelectedIndices[0]);
                SaveArchiveSets();
                ShowSets();
            }
        }

        private void btnExecuteSet_Click(object sender, EventArgs e)
        {
            ArchiveSet set = this.sets[lvwSets.SelectedIndices[0]];
            ArchiveProgressForm dialog = new ArchiveProgressForm(set);
            dialog.Owner = this;
            try
            {
                this.Visible = false;
                dialog.ShowDialog();
            }
            finally
            {
                this.Visible = true;
            }
        }

        private void LoadArchiveSets()
        {
            this.sets = new List<ArchiveSet>();

            if (File.Exists(Path.Combine(GetConfigDir(), "archivesets.json")))
            {
                string str = File.ReadAllText(Path.Combine(GetConfigDir(), "archivesets.json"));

                JObject jRoot = JObject.Parse(str);

                JArray jSetsArray = (JArray)jRoot.GetValue("ArchiveSets");
                for (int i = 0; i < jSetsArray.Count; i++)
                {
                    JObject jSet = (JObject)jSetsArray[i];
                    this.sets.Add(new ArchiveSet(jSet));
                }
            }
        }

        private void SaveArchiveSets()
        {
            JArray jSetsArray = new JArray();
            foreach(ArchiveSet set in this.sets)
            {
                jSetsArray.Add(set.ToJObject());
            }

            JObject jRoot = new JObject();
            jRoot.Add("ArchiveSets", jSetsArray);

            Directory.CreateDirectory(GetConfigDir());
            File.WriteAllText(Path.Combine(GetConfigDir(), "archivesets.json"), jRoot.ToString());
        }

        private string GetConfigDir()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".archiver");
        }
    }
}
