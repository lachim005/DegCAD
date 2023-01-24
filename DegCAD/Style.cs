﻿using System;
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

        public static Style Default => new Style() { Color = Colors.Black, LineStyle = 0 };

        public override bool Equals(object? obj)
        {
            if (obj is not Style s) return false;
            if (!s.Color.Equals(Color)) return false;
            if (s.LineStyle != LineStyle) return false;
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
