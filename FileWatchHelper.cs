using System;
using System.IO;

namespace CustomBeatmaps
{
    public static class FileWatchHelper
    {

        public static void WatchFile(string fpath, Action onChange)
        {
            var fileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(fpath),
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
                               NotifyFilters.FileName
                               | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size,
                EnableRaisingEvents = true,
                Filter = fpath,
                IncludeSubdirectories = false
            };
            fileWatcher.Changed += (sender, args) => onChange.Invoke();
            fileWatcher.Created += (sender, args) => onChange.Invoke();
            fileWatcher.Deleted += (sender, args) => onChange.Invoke();
            fileWatcher.Renamed += (sender, args) => onChange.Invoke();
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
            WatchFile(fpath, () =>
            {
                if (File.Exists(fpath))
                {
                    onWriteChange.Invoke(fpath);
                }
            });

        }
    }
}
