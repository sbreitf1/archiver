using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Archiver
{
    class ArchiveSet
    {
        private const string ArchiveMetaDir = ".archive-meta";
        private const string ArchiveMetaLastStatFile = "last-stat";
        private const string ArchiveMetaSHA256File = "sha256";
        private const string ArchiveMetaLastSeenFile = "last-seen";

        public string ID { get; set; }
        public string Name { get; set; }
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
            public long TotalFileCount;
            public long TotalUpdateFileSize;
            public long TotalImportFileSize;
            public bool IsProcessing;
            public string CurrentFile;
            public long ProcessedFiles;
            public long UpdatedFileSize;
            public long ImportedFileSize;
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

            List<GatheredFile> files = new List<GatheredFile>();
            List<GatheredDir> dirs = new List<GatheredDir>();
            GatherChangedFiles(ctx, files, dirs, this.BackupDir);

            string metaDir = GetMetaPath("");
            Directory.CreateDirectory(metaDir);
            File.SetAttributes(metaDir, File.GetAttributes(metaDir) | FileAttributes.Hidden);

            foreach (GatheredDir d in dirs)
            {
                string dataDirPath = GetDataPath(d.Path);
                Directory.CreateDirectory(dataDirPath);
                try
                {
                    DirectoryInfo di = new DirectoryInfo(dataDirPath);
                    di.CreationTime = d.Created;
                    di.LastWriteTime = d.LastModified;
                    di.LastAccessTime = d.LastAccess;
                }
                catch (Exception ex)
                {
                    //TODO append error to list
                }
            }

            ctx.Status.IsProcessing = true;
            ctx.NotifyStatusChanged();

            //TODO somehow set directory timestamps

            foreach (GatheredFile f in files)
            {
                ProcessFile(ctx, f);
            }
        }

        private struct GatheredDir
        {
            public string Path;
            public DateTime Created;
            public DateTime LastModified;
            public DateTime LastAccess;
        }

        private void GatherChangedFiles(Context ctx, List<GatheredFile> files, List<GatheredDir> dirs, string dir)
        {
            foreach (string subdir in Directory.GetDirectories(dir))
            {
                DirectoryInfo di = new DirectoryInfo(subdir);
                GatheredDir gd = new GatheredDir();
                gd.Path = subdir;
                gd.Created = di.CreationTime;
                gd.LastModified = di.LastWriteTime;
                gd.LastAccess = di.LastAccessTime;
                dirs.Add(gd);

                GatherChangedFiles(ctx, files, dirs, subdir);
            }

            foreach (string file in Directory.GetFiles(dir))
            {
                ctx.Status.CurrentFile = file;
                ctx.NotifyStatusChanged();
                GatheredFile gf = GatherFile(ctx, file);
                files.Add(gf);
                ctx.Status.TotalFileCount++;
                if (gf.Action == ArchiveAction.AddOrUpdate)
                {
                    ctx.Status.TotalUpdateFileSize += gf.Size;
                }
                else if (gf.Action == ArchiveAction.Import)
                {
                    ctx.Status.TotalImportFileSize += gf.Size;
                }
                ctx.NotifyStatusChanged();
            }
        }

        private struct GatheredFile
        {
            public string Path;
            public ArchiveAction Action;
            public long Size;
            public DateTime LastModified;
        }

        private GatheredFile GatherFile(Context ctx, string path)
        {
            FileInfo fi = new FileInfo(path);
            GatheredFile gf = new GatheredFile();
            gf.Path = path;
            gf.Action = GetArchiveAction(path, fi);
            gf.Size = fi.Length;
            gf.LastModified = fi.LastWriteTime;
            return gf;
        }

        private enum ArchiveAction
        {
            /// <summary>
            /// The file is already archived and seems to be up-to-date.
            /// </summary>
            None,
            /// <summary>
            /// Copy all file contents to archive and overwrite existing data.
            /// </summary>
            AddOrUpdate,
            /// <summary>
            /// The file is already archived and seems to be up-to-date, but meta data are missing.
            /// </summary>
            Import,
        }

        private ArchiveAction GetArchiveAction(string file, FileInfo fi)
        {
            string dataPath = GetDataPath(file);
            string metaPath = GetMetaPath(file);

            FileInfo fia = new FileInfo(dataPath);
            if (fia.Exists && fia.Length == fi.Length)
            {
                // data file exists and has expected size

                // check for meta data
                if (!Directory.Exists(metaPath))
                {
                    //TODO check small sample of file ranges and last modified to be equal
                    return ArchiveAction.Import;
                }
                else
                {
                    if (!File.Exists(Path.Combine(metaPath, ArchiveMetaLastStatFile)))
                    {
                        // last-stat meta file missing
                        return ArchiveAction.AddOrUpdate;
                    }
                    string lastStatStr = File.ReadAllText(Path.Combine(metaPath, ArchiveMetaLastStatFile));
                    if (lastStatStr != FormatLastStat(File.GetLastWriteTime(file), fi.Length))
                    {
                        // file was modified since last archive
                        return ArchiveAction.AddOrUpdate;
                    }
                    if (!File.Exists(Path.Combine(metaPath, ArchiveMetaSHA256File)))
                    {
                        // sha256 meta file missing
                        return ArchiveAction.AddOrUpdate;
                    }
                    //TODO check sha256 has correct format (no actual checksum check)
                    if (!File.Exists(Path.Combine(metaPath, ArchiveMetaLastSeenFile)))
                    {
                        // last-seen meta file missing
                        return ArchiveAction.AddOrUpdate;
                    }
                    //TODO check last-seen has correct format
                    return ArchiveAction.None;
                }
            }

            // default is the safe way
            return ArchiveAction.AddOrUpdate;
        }

        private void ProcessFile(Context ctx, GatheredFile file)
        {
            if (file.Action != ArchiveAction.None)
            {
                Console.WriteLine("do " + file.Action + " on " + file.Path);
            }
            ctx.Status.CurrentFile = file.Path;
            ctx.NotifyStatusChanged();

            if (file.Action == ArchiveAction.None)
            {
                DoActionNone(ctx, file);
            }
            else if (file.Action == ArchiveAction.AddOrUpdate)
            {
                DoActionAddOrUpdate(ctx, file);
            }
            else if (file.Action == ArchiveAction.Import)
            {
                DoActionImport(ctx, file);
            }

            ctx.Status.ProcessedFiles++;
            ctx.NotifyStatusChanged();
        }

        private void DoActionNone(Context ctx, GatheredFile file)
        {
            // no data update, but remember when the file has last been seen in the source
            string metaPath = GetMetaPath(file.Path);
            File.WriteAllText(Path.Combine(metaPath, ArchiveMetaLastSeenFile), FormatLastSeen(DateTime.Now));
        }

        private void DoActionAddOrUpdate(Context ctx, GatheredFile file)
        {
            string metaPath = GetMetaPath(file.Path);
            Directory.CreateDirectory(metaPath);

            // delete old meta stuff to indicate incomplete archive of this file
            if (File.Exists(Path.Combine(metaPath, ArchiveMetaSHA256File)))
            {
                File.Delete(Path.Combine(metaPath, ArchiveMetaSHA256File));
            }
            if (File.Exists(Path.Combine(metaPath, ArchiveMetaLastStatFile)))
            {
                File.Delete(Path.Combine(metaPath, ArchiveMetaLastStatFile));
            }
            if (File.Exists(Path.Combine(metaPath, ArchiveMetaLastSeenFile)))
            {
                File.Delete(Path.Combine(metaPath, ArchiveMetaLastSeenFile));
            }

            string dataPath = GetDataPath(file.Path);

            // copy file to dst and gather meta data
            string dataDir = Path.GetDirectoryName(dataPath);
            Directory.CreateDirectory(dataDir);
            string hashStr;
            long fileSize;
            using (FileStream istream = new FileStream(file.Path, FileMode.Open))
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

                            ctx.Status.UpdatedFileSize += count;
                            ctx.NotifyStatusChanged();
                        }
                        hasher.TransformFinalBlock(new byte[] { }, 0, 0);
                        hashStr = BitConverter.ToString(hasher.Hash).ToLower().Replace("-", "");
                    }
                }
            }

            FileInfo fi = new FileInfo(file.Path);
            FileInfo fia = new FileInfo(dataPath);
            fia.CreationTime = fi.CreationTime;
            fia.LastAccessTime = fi.LastAccessTime;
            fia.LastWriteTime = fi.LastWriteTime;

            // write meta data (also indicates complete archive of this file)
            File.WriteAllText(Path.Combine(metaPath, ArchiveMetaSHA256File), hashStr);
            File.WriteAllText(Path.Combine(metaPath, ArchiveMetaLastStatFile), FormatLastStat(File.GetLastWriteTime(file.Path), fileSize));
            File.WriteAllText(Path.Combine(metaPath, ArchiveMetaLastSeenFile), FormatLastSeen(DateTime.Now));
        }

        private void DoActionImport(Context ctx, GatheredFile file)
        {
            // compute sha256 of original file
            string hashStr;
            long fileSize;
            using (FileStream istream = new FileStream(file.Path, FileMode.Open))
            {
                fileSize = istream.Length;
                using (SHA256 hasher = SHA256.Create())
                {
                    byte[] buffer = new byte[1024 * 1024];
                    while (istream.Position < istream.Length)
                    {
                        int count = istream.Read(buffer, 0, buffer.Length);
                        hasher.TransformBlock(buffer, 0, count, null, 0);

                        ctx.Status.ImportedFileSize += count;
                        ctx.NotifyStatusChanged();
                    }
                    hasher.TransformFinalBlock(new byte[] { }, 0, 0);
                    hashStr = BitConverter.ToString(hasher.Hash).ToLower().Replace("-", "");
                }
            }

            FileInfo fi = new FileInfo(file.Path);

            // write meta data (also indicates complete archive of this file)
            string metaPath = GetMetaPath(file.Path);
            Directory.CreateDirectory(metaPath);
            File.WriteAllText(Path.Combine(metaPath, ArchiveMetaSHA256File), hashStr);
            File.WriteAllText(Path.Combine(metaPath, ArchiveMetaLastStatFile), FormatLastStat(File.GetLastWriteTime(file.Path), fileSize));
            File.WriteAllText(Path.Combine(metaPath, ArchiveMetaLastSeenFile), FormatLastSeen(DateTime.Now));
        }

        private string GetDataPath(string srcPath)
        {
            if (string.IsNullOrWhiteSpace(srcPath))
            {
                return this.DestinationDir;
            }
            return Path.Combine(this.DestinationDir, srcPath.Substring(this.BackupDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        private string GetMetaPath(string srcPath)
        {
            if (string.IsNullOrWhiteSpace(srcPath))
            {
                return Path.Combine(this.DestinationDir, ArchiveMetaDir);
            }
            return Path.Combine(this.DestinationDir, ArchiveMetaDir, srcPath.Substring(this.BackupDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        private string FormatLastStat(DateTime lastModified, long size)
        {
            return lastModified.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK") + "|" + size;
        }

        private string FormatLastSeen(DateTime lastSeen)
        {
            return lastSeen.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
        }
    }
}
