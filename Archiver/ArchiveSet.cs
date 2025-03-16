using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Archiver
{
    public class ArchiveSet
    {
        private const string ArchiveMetaDir = ".archive-meta";
        private const string ArchiveSetFile = ".archiveset.json";
        private const string ArchiveMetaSHA256File = "sha256";
        private const string ArchiveLastSeenFile = ".archive-last-seen";

        public string ID { get; set; }
        public string Name { get; set; }
        public string BackupDir { get; set; }
        public string DestinationDir { get; set; }
        public List<string> ExcludedPaths { get; set; }

        public ArchiveSet()
        {
            this.ID = Guid.NewGuid().ToString();
            this.Name = "New Archive Set";
            this.ExcludedPaths = new List<string>();
        }

        public ArchiveSet(JObject jSet)
        {
            this.ID = jSet.GetValue("ID").Value<string>();
            this.Name = jSet.GetValue("Name").Value<string>();
            this.BackupDir = jSet.GetValue("LocalDir").Value<string>();
            this.DestinationDir = jSet.GetValue("DestinationDir").Value<string>();
            //TODO read excluded paths
            this.ExcludedPaths = new List<string>();
        }

        public JObject ToJObject()
        {
            JObject jSet = new JObject();
            jSet.Add("ID", this.ID);
            jSet.Add("Name", this.Name);
            jSet.Add("LocalDir", this.BackupDir);
            jSet.Add("DestinationDir", this.DestinationDir);
            //TODO write excluded paths
            return jSet;
        }

        public delegate void NotifyStatusHandler(Status status);
        public delegate void NotifyMessageHandler(string msg);

        public struct Status
        {
            public string CurrentAction;
            public long TotalFileCount;
            public long TotalUpdateFileSize;
            public long TotalImportFileSize;
            public bool IsArchiving;
            public string CurrentFile;
            public long ProcessedFiles;
            public long UpdatedFileSize;
            public long ImportedFileSize;
        }

        private class Context
        {
            public Status Status;
            private NotifyStatusHandler hStatus;
            private NotifyMessageHandler hMsg;

            public DateTime Now { get; private set; }

            public Context(NotifyStatusHandler hStatus, NotifyMessageHandler hMsg)
            {
                this.hStatus = hStatus;
                this.hMsg = hMsg;
                this.Now = DateTime.Now;
            }

            public void SetCurrentAction(string action)
            {
                this.Status.CurrentAction = action;
                NotifyStatusChanged();
            }

            public void NotifyStatusChanged()
            {
                this.hStatus.Invoke(this.Status);
            }

            public void AddErrMessage(string msg)
            {
                this.hMsg.Invoke("[ERR] " + msg);
            }
            public void AddWarnMessage(string msg)
            {
                this.hMsg.Invoke("[WARN] " + msg);
            }
        }

        public void Do(NotifyStatusHandler hStatus, NotifyMessageHandler hMsg)
        {
            Context ctx = new Context(hStatus, hMsg);

            ctx.SetCurrentAction("Build archive index");
            Dictionary<string, ArchivedFile> archivedFiles = new Dictionary<string, ArchivedFile>();
            try
            {
                CollectArchivedFiles(ctx, archivedFiles, this.DestinationDir, true);
            }
            catch (Exception ex)
            {
                ctx.AddErrMessage("Failed to read archived files: " + ex.Message);
                throw ex;
            }
            Console.WriteLine(archivedFiles.Count + " files already in archive dir");

            ctx.SetCurrentAction("Prepare last seen index");
            Dictionary<string, DateTime> archiveLastSeen = new Dictionary<string, DateTime>();
            foreach (ArchivedFile af in archivedFiles.Values)
            {
                archiveLastSeen[GetRelPathFromDst(af.Path).ToLower()] = ctx.Now;
            }

            string lastSeenFile = Path.Combine(this.DestinationDir, ArchiveLastSeenFile);
            try
            {
                if (File.Exists(lastSeenFile))
                {
                    string[] lines = File.ReadAllLines(lastSeenFile);
                    int parseFailCount = 0;
                    foreach (string l in lines)
                    {
                        string[] parts = l.Split('|');
                        string fileName = string.Join("|", parts, 0, parts.Length - 1);
                        try
                        {
                            DateTime lastSeen = ParseLastSeen(parts[parts.Length - 1]);
                            archiveLastSeen[fileName.ToLower()] = lastSeen;
                        }
                        catch
                        {
                            parseFailCount++;
                        }
                    }
                    if (parseFailCount > 0)
                    {
                        ctx.AddWarnMessage("Failed to parse " + parseFailCount + " last seen time(s)");
                    }
                }
            }
            catch (Exception ex)
            {
                ctx.AddWarnMessage("Failed read " + ArchiveLastSeenFile + ": " + ex.Message);
            }

            ctx.SetCurrentAction("Gather source files to archive");
            List<GatheredFile> files = new List<GatheredFile>();
            List<GatheredDir> dirs = new List<GatheredDir>();
            GatherChangedFiles(ctx, archivedFiles, files, dirs, this.BackupDir, true);
            Console.WriteLine("found " + files.Count + " files and " + dirs.Count + " directories in source directory");

            ctx.SetCurrentAction("Archive files");
            string metaDir = GetMetaPath("");
            try
            {
                Directory.CreateDirectory(metaDir);
            }
            catch (Exception ex)
            {
                ctx.AddErrMessage("Failed to prepare " + ArchiveMetaDir + " directory: " + ex.Message);
                throw ex;
            }
            try
            {
                File.SetAttributes(metaDir, File.GetAttributes(metaDir) | FileAttributes.Hidden);
            }
            catch (Exception ex)
            {
                ctx.AddErrMessage("Failed to set " + ArchiveMetaDir + " directory attributes: " + ex.Message);
                throw ex;
            }

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
                    ctx.AddWarnMessage("Failed to set directory attributes of " + d.Path + ": " + ex.Message);
                }
            }

            ctx.Status.IsArchiving = true;
            ctx.NotifyStatusChanged();

            foreach (GatheredFile f in files)
            {
                ProcessFile(ctx, f);
                archiveLastSeen[GetRelPathFromSrc(f.Path).ToLower()] = ctx.Now;
            }

            try
            {
                List<string> outLines = new List<string>();
                foreach (KeyValuePair<string, DateTime> kvp in archiveLastSeen)
                {
                    outLines.Add(kvp.Key + "|" + FormatLastSeen(kvp.Value));
                }
                if (File.Exists(lastSeenFile))
                {
                    File.Delete(lastSeenFile);
                }
                File.WriteAllLines(lastSeenFile, outLines.ToArray());
            }
            catch (Exception ex)
            {
                ctx.AddWarnMessage("Failed to write " + ArchiveLastSeenFile + ": " + ex.Message);
            }
            try
            {
                File.SetAttributes(lastSeenFile, File.GetAttributes(lastSeenFile) | FileAttributes.Hidden);
            }
            catch (Exception ex)
            {
                ctx.AddWarnMessage("Failed to set file attributes of " + ArchiveLastSeenFile + ": " + ex.Message);
            }

            string clonedArchiveSetFile = Path.Combine(this.DestinationDir, ArchiveSetFile);
            try
            {
                JObject jSet = ToJObject();
                if (File.Exists(clonedArchiveSetFile))
                {
                    File.Delete(clonedArchiveSetFile);
                }
                File.WriteAllText(clonedArchiveSetFile, jSet.ToString());
            }
            catch (Exception ex)
            {
                ctx.AddWarnMessage("Failed to export " + ArchiveSetFile + ": " + ex.Message);
            }
            try
            {
                File.SetAttributes(clonedArchiveSetFile, File.GetAttributes(clonedArchiveSetFile) | FileAttributes.Hidden);
            }
            catch (Exception ex)
            {
                ctx.AddWarnMessage("Failed to set file attributes of " + ArchiveSetFile + ": " + ex.Message);
            }
        }

        private struct ArchivedFile
        {
            public string Path;
            public long Size;
            public DateTime Created;
            public DateTime LastModified;
            public DateTime LastAccess;
        }

        private void CollectArchivedFiles(Context ctx, Dictionary<string, ArchivedFile> archivedFiles, string dir, bool isRoot)
        {
            foreach (string subdir in Directory.GetDirectories(dir))
            {
                if (!isRoot || !Path.GetFileName(subdir).Equals(ArchiveMetaDir, StringComparison.InvariantCultureIgnoreCase))
                {
                    CollectArchivedFiles(ctx, archivedFiles, subdir, false);
                }
            }

            foreach (string file in Directory.GetFiles(dir))
            {
                if (!isRoot || !Path.GetFileName(file).Equals(ArchiveLastSeenFile, StringComparison.InvariantCultureIgnoreCase))
                {
                    FileInfo fi = new FileInfo(file);
                    ArchivedFile af = new ArchivedFile();
                    af.Path = file;
                    af.Size = fi.Length;
                    af.Created = fi.CreationTime;
                    af.LastModified = fi.LastWriteTime;
                    af.LastAccess = fi.LastAccessTime;
                    archivedFiles.Add(file.ToLower(), af);
                }
            }
        }

        private struct GatheredDir
        {
            public string Path;
            public DateTime Created;
            public DateTime LastModified;
            public DateTime LastAccess;
        }

        private void GatherChangedFiles(Context ctx, Dictionary<string, ArchivedFile> archivedFiles, List<GatheredFile> files, List<GatheredDir> dirs, string dir, bool isRoot)
        {
            foreach (string subdir in Directory.GetDirectories(dir))
            {
                if (isRoot && IsReservedName(Path.GetFileName(subdir)))
                {
                    ctx.AddWarnMessage("Directory " + subdir + " cannot be archived. It is using a reserved name");
                    continue;
                }

                DirectoryInfo di = new DirectoryInfo(subdir);
                GatheredDir gd = new GatheredDir();
                gd.Path = subdir;
                gd.Created = di.CreationTime;
                gd.LastModified = di.LastWriteTime;
                gd.LastAccess = di.LastAccessTime;
                dirs.Add(gd);

                GatherChangedFiles(ctx, archivedFiles, files, dirs, subdir, false);
            }

            foreach (string file in Directory.GetFiles(dir))
            {
                if (isRoot && IsReservedName(Path.GetFileName(file)))
                {
                    ctx.AddWarnMessage("File " + file + " cannot be archived. It is using a reserved name");
                    continue;
                }

                ctx.Status.CurrentFile = file;
                ctx.NotifyStatusChanged();
                GatheredFile gf = GatherFile(ctx, archivedFiles, file);
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

        private GatheredFile GatherFile(Context ctx, Dictionary<string, ArchivedFile> archivedFiles, string path)
        {
            FileInfo fi = new FileInfo(path);
            GatheredFile gf = new GatheredFile();
            gf.Path = path;
            gf.Action = GetArchiveAction(archivedFiles, path, fi);
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

        private ArchiveAction GetArchiveAction(Dictionary<string, ArchivedFile> archivedFiles, string file, FileInfo fi)
        {
            string dataPath = GetDataPath(file);
            if (archivedFiles.ContainsKey(dataPath.ToLower()))
            {
                // data file exists

                ArchivedFile af = archivedFiles[dataPath.ToLower()];
                if (af.LastModified == fi.LastWriteTime)
                {
                    // data file exists and has expected size and attributes


                    // check for meta data
                    string metaPath = GetMetaPath(file);
                    if (!Directory.Exists(metaPath))
                    {
                        // not meta-data exist, check if the existing file can be used as archived file

                        //TODO check small sample of file ranges to be equal
                        return ArchiveAction.Import;
                    }
                    else
                    {
                        if (!File.Exists(Path.Combine(metaPath, ArchiveMetaSHA256File)))
                        {
                            // sha256 meta file missing
                            return ArchiveAction.AddOrUpdate;
                        }
                        //TODO check sha256 has correct format (no actual checksum check)

                        // data file seems to be equal to source file
                        return ArchiveAction.None;
                    }
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

            // write meta data (also indicates complete archive of this file)
            string metaPath = GetMetaPath(file.Path);
            Directory.CreateDirectory(metaPath);
            File.WriteAllText(Path.Combine(metaPath, ArchiveMetaSHA256File), hashStr);
        }

        private string GetRelPathFromSrc(string srcPath)
        {
            return srcPath.Substring(this.BackupDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        private string GetRelPathFromDst(string dstPath)
        {
            return dstPath.Substring(this.DestinationDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        private string GetDataPath(string srcPath)
        {
            if (string.IsNullOrWhiteSpace(srcPath))
            {
                return this.DestinationDir;
            }
            return Path.Combine(this.DestinationDir, GetRelPathFromSrc(srcPath));
        }

        private string GetMetaPath(string srcPath)
        {
            if (string.IsNullOrWhiteSpace(srcPath))
            {
                return Path.Combine(this.DestinationDir, ArchiveMetaDir);
            }
            return Path.Combine(this.DestinationDir, ArchiveMetaDir, GetRelPathFromSrc(srcPath));
        }

        private DateTime ParseLastSeen(string str)
        {
            return DateTime.ParseExact(str, "yyyy-MM-dd'T'HH:mm:ss.fffK", null);
        }

        private string FormatLastSeen(DateTime lastSeen)
        {
            return lastSeen.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK");
        }

        private bool IsReservedName(string fileName)
        {
            return fileName.Equals(ArchiveMetaDir, StringComparison.InvariantCultureIgnoreCase) || fileName.Equals(ArchiveLastSeenFile, StringComparison.InvariantCultureIgnoreCase) || fileName.Equals(ArchiveSetFile, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool DirContainsArchive(string dir)
        {
            return Directory.Exists(Path.Combine(dir, ArchiveMetaDir)) && File.Exists(Path.Combine(dir, ArchiveSetFile));
        }

        public static string GetArchiveSetID(string dir)
        {
            string str = File.ReadAllText(Path.Combine(dir, ArchiveSetFile));
            JObject jSet = JObject.Parse(str);
            return jSet["ID"].Value<string>();
        }
    }
}
