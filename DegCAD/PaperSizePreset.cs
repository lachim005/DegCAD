using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD
{
    public class PaperSizePreset
    {
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
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
    }
}
