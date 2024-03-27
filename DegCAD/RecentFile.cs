using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public class RecentFile
    {
        public string Path { get; init; }
        public DateTime TimeOpen { get; set; }
        public FileType FileType { get; init; }


        public RecentFile(string path, FileType fileType) : this(path, fileType, DateTime.Now) { }

        public RecentFile(string path, FileType fileType, DateTime time)
        {
            Path = path;
            TimeOpen = time;
            FileType = fileType;
        }
    }
}
