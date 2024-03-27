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
        public const int MaxRecentFiles = 20;
        public ObservableCollection<RecentFile> Files { get; init; }
        public event EventHandler RecentFilesChanged;

        public void AddFile(string path, FileType fileType)
        {
            for (int i = 0; i < Files.Count; i++)
            {
                var file = Files[i];
                if (file.Path == path)
                {
                    file.TimeOpen = DateTime.Now;
                    Files.Move(i, 0);
                    RecentFilesChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }

            Files.Insert(0, new(path, fileType));
            RecentFilesChanged?.Invoke(this, EventArgs.Empty);
        }
        public void AddFile(RecentFile rf)
        {
            for (int i = 0; i < Files.Count; i++)
            {
                var file = Files[i];
                if (file.Path == rf.Path)
                {
                    file.TimeOpen = DateTime.Now;
                    Files.Move(i, 0);
                    RecentFilesChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }

            Files.Insert(0, rf);
            RecentFilesChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveFile(string path)
        {
            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].Path != path) continue;
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
