using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IOPath = System.IO.Path;

namespace DegCAD
{
    public class RecentFile : INotifyPropertyChanged
    {
        private DateTime _timeOpen;
        public string Path { get; init; }
        public string Name => IOPath.GetFileNameWithoutExtension(Path);
        public DateTime TimeOpen
        {
            get => _timeOpen;
            set
            {
                _timeOpen = value;
                PropertyChanged?.Invoke(this, new(nameof(TimeOpen)));
                PropertyChanged?.Invoke(this, new(nameof(LocalisedTimeOpen)));
            }
        }
        public FileType FileType { get; init; }
        public string Icon => FileType switch
        {
            FileType.Plane => "M 0 20 L 40 20 M 20 0 L 20 40",
            FileType.Monge => "M 0 20 L 40 20 M 10 20 L 25 40 M 30 20 L 15 0",
            FileType.Axonometry => "M 20 0 L 20 25 L 0 40 M 20 25 L 40 40",
            FileType.MultiFile => "M 0 0 L 40 0 L 40 17 L 0 17 z M 0 23 L 17 23 L 17 40 L 0 40 z M 23 23 L 40 23 L 40 40 L 23 40 z",
            _ => "M 0 0 L 40 40",
        };
        public string LocalisedTimeOpen => TimeOpen.ToShortDateString() + " " + TimeOpen.ToShortTimeString();
        public Visibility Visibility { get; set; } = Visibility.Visible;

        public RecentFile(string path, FileType fileType) : this(path, fileType, DateTime.Now) { }

        public RecentFile(string path, FileType fileType, DateTime time)
        {
            Path = path;
#if RELEASE_PORTABLE
            // Checks if the file is on the same drive as the app
            if (IOPath.GetFullPath(path)[0] == AppContext.BaseDirectory[0])
            {
                Path = IOPath.GetRelativePath(AppContext.BaseDirectory, path);
            }
#endif
            TimeOpen = time;
            FileType = fileType;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool PathEquals(string path)
        {
            return IOPath.GetFullPath(path) == IOPath.GetFullPath(Path);
        }
    }
}
