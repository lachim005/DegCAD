using System.Collections.Generic;

namespace DegCAD.DrawableItems
{
    internal class Point : IMongeItem
    {
        public double X { get; init; }
        public double Y { get; init; }
        public Vector2 Coords => new Vector2(X, Y);
        public Style Style { get; init; } = Style.Default;
        public Vector2[] SnapablePoints { get; init; }
        public ParametricLine2[] SnapableLines { get; } = new ParametricLine2[0];
        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Point(double x, double y) : this(x, y, Style.Default) { }
        public Point(double x, double y, Style style)
        {
            X = x;
            Y = y;
            Style = style;

            //Adds snapable points
            SnapablePoints = new Vector2[1] { new(X, Y) };
        }

        public void Draw(ViewportLayer gd)
        {
            Draw(gd, Style);
        }
        public void Draw(ViewportLayer gd, Style s)
        {
            gd.DrawPointCross((X, Y), s);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {

        }
    }
}
