using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Archiver
{
    class ArchiveSet
    {
        public string BackupDir { get; set; }
        public string DestinationDir { get; set; }
        public List<string> ExcludedPaths { get; set; }

        public ArchiveSet()
        {
            this.ExcludedPaths = new List<string>();
        }

        public delegate void NotifyStatusHandler(Status status);

        public struct Status
        {
            public long TotalChangedFileCount;
            public long TotalChangedFileSize;
            public bool IsProcessing;
            public string CurrentFile;
            public long ProcessedFiles;
            public long ProcessedFileSize;
        }

        private class Context
        {
            public Status Status;
            private NotifyStatusHandler h;

            public Context(NotifyStatusHandler h)
            {
                this.h = h;
            }

            public void NotifyStatusChanged()
            {
                h.Invoke(this.Status);
            }
        }

        public void Do(NotifyStatusHandler h)
        {
            Context ctx = new Context(h);

            List<string> files = new List<string>();
            GatherChangedFiles(ctx, files, this.BackupDir);

            Directory.CreateDirectory(Path.Combine(this.DestinationDir, ".data"));
            Directory.CreateDirectory(Path.Combine(this.DestinationDir, ".meta"));

            ctx.Status.IsProcessing = true;
            ctx.NotifyStatusChanged();

            //TODO somehow set directory timestamps

            foreach (string f in files)
            {
                ArchiveFile(ctx, f);
            }
        }

        private void GatherChangedFiles(Context ctx, List<string> files, string dir)
        {
            foreach (string subdir in Directory.GetDirectories(dir))
            {
                GatherChangedFiles(ctx, files, subdir);
            }

            foreach (string file in Directory.GetFiles(dir))
            {
                ctx.Status.CurrentFile = file;
                ctx.NotifyStatusChanged();
                if (FileNeedsArchive(file))
                {
                    files.Add(file);
                    FileInfo fi = new FileInfo(file);
                    ctx.Status.TotalChangedFileCount++;
                    ctx.Status.TotalChangedFileSize += fi.Length;
                    ctx.NotifyStatusChanged();
                }
            }
        }

        private bool FileNeedsArchive(string file)
        {
            // check meta exists and is intact
            string metaPath = GetMetaPath(file);
            if (!Directory.Exists(metaPath))
            {
                // no meta files in archive
                return true;
            }

            FileInfo fi = new FileInfo(file);

            if (!File.Exists(Path.Combine(metaPath, "last-stat")))
            {
                // last-stat meta file missing
                return true;
            }
            string lastStatStr = File.ReadAllText(Path.Combine(metaPath, "last-stat"));
            if (lastStatStr != File.GetLastWriteTime(file).ToString("yyyy-MM-dd'T'HH:mm:ss.fffK") + "|" + fi.Length)
            {
                // file was modified since last archive
                return true;
            }

            //TODO check sha256 exists and has correct format
            //TODO check last-seen exists and has correct format

            string dataPath = GetDataPath(file);
            if (!File.Exists(dataPath))
            {
                // data file missing in archive
                return true;
            }
            FileInfo afi = new FileInfo(dataPath);
            if (afi.Length != fi.Length)
            {
                // file size of archived file differs
                return true;
            }

            return false;
        }

        private void ArchiveFile(Context ctx, string file)
        {
            Console.WriteLine("archive " + file);
            ctx.Status.CurrentFile = file;
            ctx.NotifyStatusChanged();

            //TODO delete old meta stuff to indicate incomplete archive of this file

            string dataPath = GetDataPath(file);

            // copy file to dst and gather meta data
            string dataDir = Path.GetDirectoryName(dataPath);
            Directory.CreateDirectory(dataDir);
            string hashStr;
            long fileSize;
            using (FileStream istream = new FileStream(file, FileMode.Open))
            {
                fileSize = istream.Length;
                using (FileStream ostream = new FileStream(dataPath, FileMode.Create))
                {
                    ostream.SetLength(istream.Length);
                    using (SHA256 hasher = SHA256.Create())
                    {
                        byte[] buffer = new byte[1024 * 1024];
                        while (ostream.Position < istream.Length)
                        {
                            int count = istream.Read(buffer, 0, buffer.Length);
                            hasher.TransformBlock(buffer, 0, count, null, 0);
                            ostream.Write(buffer, 0, count);

                            ctx.Status.ProcessedFileSize += count;
                            ctx.NotifyStatusChanged();
                        }
                        hasher.TransformFinalBlock(new byte[] { }, 0, 0);
                        hashStr = BitConverter.ToString(hasher.Hash).ToLower().Replace("-", "");
                    }
                }
            }

            FileInfo fi = new FileInfo(file);
            FileInfo fia = new FileInfo(dataPath);
            fia.CreationTime = fi.CreationTime;
            fia.LastAccessTime = fi.LastAccessTime;
            fia.LastWriteTime = fi.LastWriteTime;

            // write meta data (also indicates complete archive of this file)
            string metaPath = GetMetaPath(file);
            Directory.CreateDirectory(metaPath);
            File.WriteAllText(Path.Combine(metaPath, "sha256"), hashStr);
            File.WriteAllText(Path.Combine(metaPath, "last-stat"), File.GetLastWriteTime(file).ToString("yyyy-MM-dd'T'HH:mm:ss.fffK") + "|" + fileSize);
            File.WriteAllText(Path.Combine(metaPath, "last-seen"), DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK"));

            ctx.Status.ProcessedFiles++;
            ctx.NotifyStatusChanged();
        }

        private string GetDataPath(string srcPath)
        {
            return Path.Combine(this.DestinationDir, ".data", srcPath.Substring(this.BackupDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        private string GetMetaPath(string srcPath)
        {
            return Path.Combine(this.DestinationDir, ".meta", srcPath.Substring(this.BackupDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }
    }
}
