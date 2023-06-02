using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace DegCAD
{
    public struct Style
    {
        public Color Color { get; set; }
        public int LineStyle { get; set; }
        public int Thickness { get; set; }

        public static Style Default => new Style() { Color = Colors.Black, LineStyle = 0 };
        public static Style HighlightStyle => new Style() { Color = Colors.Red, Thickness = 3 };
        public static Style BlueDashStyle => new() { Color = Color.FromRgb(0, 0, 255), LineStyle = 1 };
        public static Style GreenStyle => new Style() { Color = Colors.YellowGreen, Thickness = 1 };

        public static double[][] StrokeDashArrays = new double[3][]
        {
            new double[0],
            new double[2] {15, 15},
            new double[4] {15, 7, 2, 7}
        };

        public override bool Equals(object? obj)
        {
            if (obj is not Style s) return false;
            if (!s.Color.Equals(Color)) return false;
            if (s.LineStyle != LineStyle) return false;
            if (s.Thickness != Thickness) return false;
            return true;
        }

        public static bool operator ==(Style left, Style right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Style left, Style right)
        {
            return !(left == right);
        }

        public override int GetHashCode() => HashCode.Combine(Color, LineStyle);
    }
}
