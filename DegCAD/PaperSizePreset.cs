using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD
{
    public class PaperSizePreset : INotifyPropertyChanged
    {
        private double _width;
        private double _height;

        public string Name { get; set; }
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                PropertyChanged?.Invoke(this, new(nameof(Width)));
            }
        }
        public double Height
        {
            get
            { 
                return _height;
            }
            set
            {
                _height = value;
                PropertyChanged?.Invoke(this, new(nameof(Height)));
            }
        }
        public Visibility ShowFold
        {
            get
            {
                double ratio = Width / Height;
                if (ratio < 1)
                    ratio = 1 / ratio;
                return (ratio < 5) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public PaperSizePreset(string name, double width, double height)
        {
            Name = name;
            Width = width;
            Height = height;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
