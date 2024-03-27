using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public class RecentFiles
    {
        public const int MaxRecentFiles = 30;
        public ObservableCollection<RecentFile> Files { get; init; }
        public event EventHandler? RecentFilesChanged;

        public void AddFile(string path, FileType fileType)
        {
            for (int i = 0; i < Files.Count; i++)
            {
                var file = Files[i];
                if (file.PathEquals(path))
                {
                    file.TimeOpen = DateTime.Now;
                    Files.Move(i, 0);
                    RecentFilesChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }

            Files.Insert(0, new(path, fileType));
            if (Files.Count > MaxRecentFiles)
            {
                Files.RemoveAt(Files.Count - 1);
            }
            RecentFilesChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveFile(string path)
        {
            for (int i = 0; i < Files.Count; i++)
            {
                if (!Files[i].PathEquals(path)) continue;
                Files.RemoveAt(i);
                RecentFilesChanged?.Invoke(this, EventArgs.Empty);
                return;
            }
        }

        public RecentFiles() : this(new(MaxRecentFiles + 1))
        {
            
        }

        public RecentFiles(List<RecentFile> recentFiles)
        {
            Files = new(recentFiles);
        }
    }
}
