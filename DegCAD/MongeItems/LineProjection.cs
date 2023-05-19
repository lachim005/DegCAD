using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace DegCAD.MongeItems
{
    /// <summary>
    /// Projection of a line in one plane
    /// </summary>
    public class LineProjection : IMongeItem
    {
        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public ParametricLine2[] SnapableLines { get; init; }

        public ParametricLine2 Line { get; init; }
        private int infinitySign;

        public bool Plane { get; init; }

        public Style Style { get; init; }

        public LineProjection(ParametricLine2 line, bool plane, Style style)
        {
            Line = line;
            Plane = plane;

            //Calculates the infinity sign for drawing the line
            infinitySign = 1;
            if (Line.DirectionVector.Y * Line.DirectionVector.X < 0)
            {
                infinitySign = -1;
            }

            if (plane)
                infinitySign *= -1;

            _line.SetStyle(style);

            SnapableLines = new ParametricLine2[1] { line };
            Style = style;
        }


        private readonly Line _line = new();

        public void Draw(ViewportLayer vpl)
        {
            Draw(vpl, Style);
        }
        public void Draw(ViewportLayer vpl, Style s)
        {
            _line.SetParaLine(vpl, Line, Line.GetParamFromY(0), double.PositiveInfinity * infinitySign);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_line);
        }
    }
}
