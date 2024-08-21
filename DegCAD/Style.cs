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

        public static Style Default => new Style() { Color = App.Skin == Skin.Light ? Colors.Black : Colors.White, LineStyle = 0 };
        public static Style HighlightStyle => new Style() { Color = Colors.Crimson, Thickness = 3 };
        public static Style BlueDashStyle => new() { Color = Color.FromRgb(15, 112, 183), LineStyle = 1 };
        public static Style GreenStyle => new Style() { Color = Colors.YellowGreen, Thickness = 1 };

        public static readonly double[][][] strokeDashArrays =
        [
            [
                [],
                [15],
                [15, 7, 2, 7]
            ],
            [
                [],
                [8],
                [8, 4, 1, 4]
            ],
            [
                [],
                [5],
                [5, 3, 0, 3]
            ],
            [
                [],
                [4],
                [4, 3, 0, 3]
            ]
        ];

        public readonly double[] StrokeDashArray => strokeDashArrays[Math.Clamp(Thickness, 0, 3)][Math.Clamp(LineStyle, 0, 2)];

        public Style()
        {
            
        }
        public Style(Style basedOn) 
        {
            Color = basedOn.Color;
            LineStyle = basedOn.LineStyle;
            Thickness = basedOn.Thickness;
        }

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
        public string ToSvgParameters()
        {
            string pars = $"stroke=\"{ToHex(Color)}\" " +
                $"stroke-width=\"{Thickness + 1}\" " +
                $"stroke-linejoin=\"round\" " +
                $"stroke-linecap=\"round\"";

            if (LineStyle > 0)
            {
                var dashArray = StrokeDashArray;

                pars += $" stroke-dasharray=\" ";
                foreach (var dash in dashArray)
                {
                    pars += (dash * (Thickness + 1)).ToString();
                    pars += ' ';
                }
                pars += '"';
            }

            return pars;
        }
        public static string ToHex(Color color)
        {
            var hex = "#";
            hex += color.R.ToString("x2");
            hex += color.G.ToString("x2");
            hex += color.B.ToString("x2");
            return hex;
        }
    }
}
