namespace MuziqGrabber.Core.DuplicationChecking {
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;

    public sealed class DirectoryCheckStrategy : ICheckStrategy {
        private readonly ICollection<DirectoryInfo> _directories;

        public DirectoryCheckStrategy(IEnumerable<string> paths) {
            _directories = new Collection<DirectoryInfo>();

            foreach (var path in paths)
                _directories.Add(new DirectoryInfo(path));
        }

        public bool IsDuplicate(string fileName) {
            foreach (var directory in _directories) {
                var matchedFiles = directory.GetFiles(fileName, SearchOption.AllDirectories);
                if (matchedFiles.Any())
                    return true;

                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                var matchedDirectories = directory.GetDirectories(fileNameWithoutExtension, SearchOption.AllDirectories);
                if (matchedDirectories.Any())
                    return true;
            }

            return false;
        }
    }
}