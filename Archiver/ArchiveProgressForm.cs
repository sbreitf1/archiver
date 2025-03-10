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
        Thread workerThread;

        public ArchiveProgressForm()
        {
            InitializeComponent();

            this.archiveSet = new ArchiveSet();
            this.archiveSet.BackupDir = @"G:\Backup\Savegames";
            //this.archiveSet.BackupDir = @"G:\Backup\Archiver Test\src";
            this.archiveSet.DestinationDir = @"G:\Backup\Archiver Test\dst";
            this.archiveSet.ExcludedPaths.Add(@"G:\Backup\Archiver Test\src\pictures\signal-2023-04-29-17-37-42-876.jpg");
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (this.isRunning)
            {
                //TODO abort
            }
            else
            {
                btnStartStop.Text = "Abort";
                pbrProgress.Style = ProgressBarStyle.Marquee;
                pbrProgress.Maximum = 100;
                pbrProgress.Value = 0;

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
                    if( status.IsProcessing)
                    {
                        lblStatus.Text = "Current File: " + (string.IsNullOrWhiteSpace(status.CurrentFile) ? "-" : status.CurrentFile) + "\r\nFiles: " + status.ProcessedFiles + " / " + status.TotalChangedFileCount + "\r\nSize: " + HumanReadableSize(status.ProcessedFileSize) + " / " + HumanReadableSize(status.TotalChangedFileSize);
                        if(pbrProgress.Style != ProgressBarStyle.Continuous)
                        {
                            pbrProgress.Style = ProgressBarStyle.Continuous;
                            pbrProgress.Maximum = 100000;
                        }
                        const double fileWeight = 65536;
                        double maxVal = (double)status.TotalChangedFileSize + fileWeight * (double)status.TotalChangedFileCount;
                        double currentVal = (double)status.ProcessedFileSize + fileWeight * (double)status.ProcessedFiles;
                        pbrProgress.Value = Math.Min(pbrProgress.Maximum, (int)((double)pbrProgress.Maximum * currentVal / maxVal));
                    }
                    else
                    {
                        lblStatus.Text = "Current File: " + (string.IsNullOrWhiteSpace(status.CurrentFile) ? "-" : status.CurrentFile) + "\r\nFiles: " + status.TotalChangedFileCount + "\r\nSize: " + HumanReadableSize(status.TotalChangedFileSize);
                    }
                }));
                this.lastNotifyTick = Environment.TickCount;
            }
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
                lblStatus.Text = "Current File: -\r\nFiles: -\r\nSize: -";
                pbrProgress.Style = ProgressBarStyle.Continuous;
                pbrProgress.Maximum = 100;
                pbrProgress.Value = 0;
            }));
        }

        private void progress()
        {
            try
            {
                this.archiveSet.Do((status) => { this.NotifyStatus(status); });

            }
            /*catch (Exception ex)
            {
                this.Invoke((MethodInvoker)(() => {
                    MessageBox.Show("Archiving failed: " + ex.Message + "\n\n" + ex.StackTrace, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }*/
            finally
            {
                this.NotifyProgressEnded();
            }
        }
    }
}
