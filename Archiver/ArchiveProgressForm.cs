using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archiver
{
    public partial class ArchiveProgressForm: Form
    {
        ArchiveSet archiveSet;
        bool isRunning;
        DateTime startTime;
        Thread workerThread;

        public ArchiveProgressForm()
        {
            InitializeComponent();

            this.archiveSet = new ArchiveSet();
            //this.archiveSet.BackupDir = @"G:\Backup\Savegames";
            this.archiveSet.BackupDir = @"G:\Backup\Archiver Test\src";
            this.archiveSet.DestinationDir = @"G:\Backup\Archiver Test\dst";
            this.archiveSet.ExcludedPaths.Add(@"G:\Backup\Archiver Test\src\pictures\signal-2023-04-29-17-37-42-876.jpg");
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (this.isRunning)
            {
                if (MessageBox.Show("Do you really want to cancel the current action?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    //TODO abort
                }
            }
            else
            {
                this.isRunning = true;
                btnStartStop.Text = "Abort";
                pbrProgress.Style = ProgressBarStyle.Marquee;
                pbrProgress.Maximum = 100;
                pbrProgress.Value = 0;
                gbxProgress.Enabled = true;
                gbxEvents.Enabled = true;
                tbxEvents.Text = "";
                this.startTime = DateTime.Now;

                this.workerThread = new Thread(new ThreadStart(this.progress));
                this.workerThread.IsBackground = true;
                this.workerThread.Start();
            }
        }

        private int lastNotifyTick;

        private void NotifyStatus(ArchiveSet.Status status)
        {
            if ((Environment.TickCount - this.lastNotifyTick) > 10)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    TimeSpan elapsedTime = (DateTime.Now - this.startTime);

                    if (string.IsNullOrWhiteSpace(status.CurrentAction))
                    {
                        gbxProgress.Text = "Progress";
                    }
                    else
                    {
                        gbxProgress.Text = "Progress [" + status.CurrentAction + "]";
                    }

                    //TODO shorten path?
                    lblCurrentFile.Text = "Current File: " + (string.IsNullOrWhiteSpace(status.CurrentFile) ? "-" : status.CurrentFile);
                    if (status.IsArchiving)
                    {
                        lblStatus.Text = "Files: " + status.ProcessedFiles + " / " + status.TotalFileCount + "\r\nSize: " + HumanReadableSize(status.UpdatedFileSize + status.ImportedFileSize) + " / " + HumanReadableSize(status.TotalUpdateFileSize + status.TotalImportFileSize);
                        if (pbrProgress.Style != ProgressBarStyle.Continuous)
                        {
                            pbrProgress.Style = ProgressBarStyle.Continuous;
                            pbrProgress.Maximum = 100000;
                        }
                        // heuristic for a smooth progress bar:
                        // - every file is treated as 256k fix overhead to account for file creation
                        // - update file size is counted twice as it requires read and write operations
                        const double fileWeight = 256 * 1024;
                        double maxVal = fileWeight * (double)status.TotalFileCount + 2 * (double)status.TotalUpdateFileSize + (double)status.TotalImportFileSize;
                        double currentVal = fileWeight * (double)status.ProcessedFiles + 2 * (double)status.UpdatedFileSize + (double)status.ImportedFileSize;
                        TimeSpan? remainingTime = null;
                        if (maxVal > 0.001)
                        {
                            pbrProgress.Value = Math.Min(pbrProgress.Maximum, (int)((double)pbrProgress.Maximum * currentVal / maxVal));
                        }

                        if (currentVal > 0.001)
                        {
                            remainingTime = TimeSpan.FromSeconds((maxVal - currentVal) * (elapsedTime.TotalSeconds / currentVal));
                        }

                        lblTime.Text = "Time: " + elapsedTime.ToString(@"mm\:ss") + "\r\nETA: " + (remainingTime.HasValue ? remainingTime.Value.ToString(@"mm\:ss") : "-");
                    }
                    else
                    {
                        lblStatus.Text = "Files: " + status.TotalFileCount + "\r\nSize: " + HumanReadableSize(status.TotalUpdateFileSize + status.TotalImportFileSize);
                        lblTime.Text = "Time: " + elapsedTime.ToString(@"mm\:ss") + "\r\nETA: -";
                    }
                }));
                this.lastNotifyTick = Environment.TickCount;
            }
        }

        private void NotifyMessage(string msg)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                tbxEvents.Text += msg + "\r\n";
            }));
        }

        private string HumanReadableSize(long size)
        {
            if (size<1000)
            {
                return size + " Bytes";
            }
            else if (size<1000000)
            {
                return Math.Round(size / 1024.0, 2) + " KiB";
            }
            else if (size < 1000000000)
            {
                return Math.Round(size / (1024.0 * 1024.0), 2) + " MiB";
            }
            else if (size < 1000000000000)
            {
                return Math.Round(size / (1024.0 * 1024.0 * 1024.0), 2) + " GiB";
            }
            else 
            {
                return Math.Round(size / (1024.0 * 1024.0 * 1024.0 * 1024.0), 2) + " TiB";
            }
        }

        private void NotifyProgressEnded()
        {
            this.Invoke((MethodInvoker)(() =>
            {
                btnStartStop.Text = "Start";
                lblCurrentFile.Text = "Current File: -";
                lblStatus.Text = "Files: -\r\nSize: -";
                lblTime.Text = "Time: -\r\nETA: -";
                pbrProgress.Style = ProgressBarStyle.Continuous;
                pbrProgress.Maximum = 100;
                pbrProgress.Value = 0;
                gbxProgress.Text = "Progress";
                gbxProgress.Enabled = false;
                // gbxErrors should stay enabled so the user can scroll errors after processing
                this.isRunning = false;
            }));
        }

        private void progress()
        {
            try
            {
                this.archiveSet.Do((status) => { this.NotifyStatus(status); }, (msg) => { this.NotifyMessage(msg); });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)(() => {
                    MessageBox.Show("Archiving failed: " + ex.Message + "\n\n" + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
            finally
            {
                this.NotifyProgressEnded();
            }
        }
    }
}
