using System.IO;

namespace CustomBeatmaps
{
    public class TemporaryCopiedFile
    {

        private readonly string _relativeLocalPath;

        public TemporaryCopiedFile(string relativeLocalPath)
        {
            _relativeLocalPath = relativeLocalPath;
        }

        public string CopyNewFile(string original)
        {
            File.Copy(original, _relativeLocalPath, true);
            return _relativeLocalPath;
        }

        public void Cleanup()
        {
            if (File.Exists(_relativeLocalPath))
            {
                File.Delete(_relativeLocalPath);
            }
        }
    }
}
