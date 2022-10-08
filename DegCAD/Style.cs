using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public struct Style
    {
        public Color Color { get; set; }
        public int LineStyle { get; set; }

        public static Style Default => new Style() { Color = Colors.Black, LineStyle = 0 };
    }
}
