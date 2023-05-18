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

        public Circle2 Circle { get; init; }
        public Vector2 Center => Circle.Center;
        public double StartAngle { get; init; }
        public double EndAngle { get; init; }
        public Style Style { get; init; }

        public Arc(Vector2 center, Vector2 point, double startPoint, double endPoint, Style style)
            : this(new Circle2(center, point), startPoint, endPoint, style) { }

        public Arc(Circle2 circle, double startPoint, double endPoint, Style style)
        {
            Circle = circle;
            StartAngle = startPoint;
            EndAngle = endPoint;
            Style = style;

            SnapablePoints = new Vector2[3] {
                Circle.Center,
                circle.CalculatePointWithAngle(startPoint),
                circle.CalculatePointWithAngle(endPoint)
            };
            SnapableLines = new ParametricLine2[0];
            SnapableCircles = new Circle2[1] { circle };
        }

        public void Draw(ViewportLayer gd)
        {
            Draw(gd, Style);
        }
        public void Draw(ViewportLayer gd, Style s)
        {
            gd.DrawArc(Circle, StartAngle, EndAngle, s);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {

        }
    }
}
