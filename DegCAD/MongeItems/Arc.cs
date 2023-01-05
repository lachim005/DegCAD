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

        public Circle2[] SnapableCircles { get; init; }

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

            double radius = (center - point).Length;

            SnapablePoints = new Vector2[3] {
                center,
                CalculateEdgePoint(startPoint, radius),
                CalculateEdgePoint(endPoint, radius),
            };
            SnapableLines = new ParametricLine2[0];
            SnapableCircles = new Circle2[1] { new Circle2(center, point) };
        }

        public void Draw(GeometryDrawer gd)
        {
            gd.DrawArc(Center, Point, StartAngle, EndAngle, Style);
        }

        private Vector2 CalculateEdgePoint(double angle, double radius)
        {
            Vector2 unitVector = Math.SinCos(angle);
            unitVector *= radius;
            return Center + (unitVector.Y, unitVector.X);
        }
    }
}
