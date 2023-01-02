using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MongeItems
{
    /// <summary>
    /// An arc defined by it's center, a point on the circle and two angles
    /// </summary>
    public class Arc : IMongeItem
    {
        public Vector2[] SnapablePoints { get; init; }

        public ParametricLine2[] SnapableLines { get; init; }

        public Vector2 Center { get; init; }
        public Vector2 Point { get; init; }
        public double StartAngle { get; init; }
        public double EndAngle { get; init; }
        public Style Style { get; init; }

        public Arc(Vector2 center, Vector2 point, double startPoint, double endPoint, Style style)
        {
            Center = center;
            Point = point;
            StartAngle = startPoint;
            EndAngle = endPoint;
            Style = style;
            SnapablePoints = new Vector2[1] { center };
            SnapableLines = new ParametricLine2[0];
        }

        public void Draw(GeometryDrawer gd)
        {
            gd.DrawArc(Center, Point, StartAngle, EndAngle, Style);
        }
    }
}
