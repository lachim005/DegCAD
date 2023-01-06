using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MongeItems
{
    public class Line : IMongeItem
    {
        public LineProjection BottomLine { get; init; }
        public LineProjection TopLine { get; init; }

        public Vector2[] SnapablePoints { get; } = new Vector2[0];
        public ParametricLine2[] SnapableLines { get; init; }
        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Style Style { get; } = Style.Default;

        public Line(ParametricLine2 botLine, ParametricLine2 topLine, Style style)
        {
            BottomLine = new(botLine, false, style);
            TopLine = new(topLine, true, style);

            SnapableLines = new ParametricLine2[2] { botLine, topLine };
            Style = style;
        }

        public void Draw(GeometryDrawer gd)
        {
            BottomLine.Draw(gd);
            TopLine.Draw(gd);
        }
    }
}
