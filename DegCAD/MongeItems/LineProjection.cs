using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD.MongeItems
{
    /// <summary>
    /// Projection of a line in one plane
    /// </summary>
    internal class LineProjection : IMongeItem
    {
        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public ParametricLine2[] SnapableLines { get; init; }

        public ParametricLine2 Line { get; init; }
        private int infinitySign;

        public Style Style { get; init; }

        public LineProjection(ParametricLine2 line, bool plane, Style style)
        {
            Line = line;

            //Calculates the infinity sign for drawing the line
            infinitySign = 1;
            if (Line.DirectionVector.Y * Line.DirectionVector.X < 0)
            {
                infinitySign = -1;
            }

            if (plane)
                infinitySign *= -1;


            SnapableLines = new ParametricLine2[1] { line };
            Style = style;
        }

        public void Draw(GeometryDrawer gd)
        {
            gd.DrawLine(Line, double.PositiveInfinity * infinitySign, Line.GetParamFromY(0), Style);
        }
    }
}
