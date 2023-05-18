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


            SnapableLines = new ParametricLine2[1] { line };
            Style = style;
        }


        public void Draw(ViewportLayer gd)
        {
            Draw(gd, Style);
        }
        public void Draw(ViewportLayer gd, Style s)
        {
            gd.DrawLine(Line, double.PositiveInfinity * infinitySign, Line.GetParamFromY(0), s);
        }
    }
}
