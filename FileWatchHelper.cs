using System;
using System.IO;

namespace CustomBeatmaps
{
    public static class FileWatchHelper
    {

        public static FileSystemWatcher WatchFile(string fpath, Action<string> onChange)
        {
            var fileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(fpath),
                NotifyFilter = NotifyFilters.DirectoryName |
                               NotifyFilters.FileName
                               | NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true,
                Filter = fpath,
                IncludeSubdirectories = false
            };

            FileSystemEventHandler Do = (sender, args) =>
            {
                string path = args.FullPath.Replace('\\', '/');
                string fullPath = Path.GetFullPath(fpath).Replace('\\', '/');
                if (path == fullPath)
                {
                    onChange.Invoke(path);
                }
            };

            fileWatcher.Changed += Do;
            fileWatcher.Created += Do;
            fileWatcher.Deleted += Do;
            fileWatcher.Renamed += (sender, args) => Do(null, args);

            return fileWatcher;
        }

        public static void WatchFolder(string dirPath, bool recursive, Action onChange)
        {
            var fileWatcher = new FileSystemWatcher
            {
                Path = dirPath,
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
                               NotifyFilters.FileName
                               | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size,
                EnableRaisingEvents = true,
                Filter = "*",
                IncludeSubdirectories = recursive
            };
            fileWatcher.Changed += (sender, args) => onChange.Invoke();
            fileWatcher.Created += (sender, args) => onChange.Invoke();
            fileWatcher.Deleted += (sender, args) => onChange.Invoke();
            fileWatcher.Renamed += (sender, args) => onChange.Invoke();
        }

        public static void WatchFileForModifications(string fpath, Action<string> onWriteChange)
        {
            WatchFile(fpath, path =>
            {
                if (File.Exists(path))
                {
                    onWriteChange.Invoke(path);
                }
            });

        }
    }
}
