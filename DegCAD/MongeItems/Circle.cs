using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MongeItems
{
    /// <summary>
    /// Circle defined by it's center and a point on it
    /// </summary>
    public class Circle : IMongeItem
    {
        public Vector2[] SnapablePoints { get; init; }
        public ParametricLine2[] SnapableLines { get; init; }
        public Circle2[] SnapableCircles { get; init; }
        public Circle2 Circle2 { get; init; }
        public Style Style { get; init; }

        public Circle(Vector2 center, Vector2 pointOnCircle, Style style)
            : this(new Circle2(center, pointOnCircle), style)
        {

        }

        public Circle(Circle2 circle, Style style)
        {
            Style = style;
            Circle2 = circle;

            SnapableLines = new ParametricLine2[0];
            SnapablePoints = new Vector2[1] { circle.Center };
            SnapableCircles = new Circle2[1] { circle };
        }

        public void Draw(GeometryDrawer gd)
        {
            gd.DrawCircle(Circle2, Style);
        }
    }
}
