namespace CustomBeatmaps.Packages
{
    public class UniqueId
    {
        // TODO: Discuss local packages w/ Jane
        private static readonly string LOCAL_PREFIX = "LOCAL_";

        public readonly string FolderPath;

        public UniqueId(string folderPath)
        {
            FolderPath = folderPath;
        }

        public bool IsPureLocal => FolderPath.StartsWith(LOCAL_PREFIX);

        // TODO: Delete Zombie code
        /*
        public static UniqueId GetRandomLocalId()
        {
            var random = new Random();
            return new UniqueId( $"{LOCAL_PREFIX}{random.Next(0x1000000):X6}");
        }
        */

        protected bool Equals(UniqueId other)
        {
            return FolderPath == other.FolderPath;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UniqueId) obj);
        }

        public override int GetHashCode()
        {
            return FolderPath != null ? FolderPath.GetHashCode() : 0;
        }

        public static bool operator ==(UniqueId left, UniqueId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UniqueId left, UniqueId right)
        {
            return !Equals(left, right);
        }
    }
}