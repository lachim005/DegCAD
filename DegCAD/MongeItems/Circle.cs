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
        public Vector2 Center { get; init; }
        public Vector2 PointOnCircle { get; init; }
        public Style Style { get; init; }

        public Circle(Vector2 center, Vector2 pointOnCircle, Style style)
        {
            PointOnCircle = pointOnCircle;
            Center = center;
            Style = style;

            SnapableLines = new ParametricLine2[0];
            SnapablePoints = new Vector2[1] { center };
        }

        public void Draw(GeometryDrawer gd)
        {
            gd.DrawCircle(Center, PointOnCircle, Style);
        }
    }
}
