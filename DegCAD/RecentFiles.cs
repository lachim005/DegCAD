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
        public ObservableCollection<RecentFile> Files { get; init; } = new();

        public void AddFile(string path, FileType fileType)
        {
            for (int i = 0; i < Files.Count; i++)
            {
                var file = Files[i];
                if (file.Path == path)
                {
                    file.TimeOpen = DateTime.Now;
                    Files.Move(i, 0);
                    return;
                }
            }

            Files.Insert(0, new(path, fileType));
        }

        public void RemoveFile(string path)
        {
            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].Path != path) continue;
                Files.RemoveAt(i);
                return;
            }
        }
    }
}
