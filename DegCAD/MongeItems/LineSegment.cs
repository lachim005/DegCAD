using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MongeItems
{
    public class LineSegment : IMongeItem
    {
        public Vector2 P1 { get; init; }
        public Vector2 P2 { get; init; }
        public Style Style { get; init; }

        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public LineSegment(Vector2 p1, Vector2 p2) : this(p1, p2, Style.Default) { }
        public LineSegment(Vector2 p1, Vector2 p2, Style style)
        {
            P1 = p1;
            P2 = p2;
            Style = style;
        }

        public void Draw(GeometryDrawer gd)
        {
            gd.DrawLine(P1, P2, Style);
        }
    }
}
