using System.Collections.Generic;

namespace DegCAD.DrawableItems
{
    internal class Point : IMongeItem
    {
        public double X { get; init; }
        public double Y { get; init; } = double.NaN;
        public double Z { get; init; } = double.NaN;
        public Style Style { get; init; } = Style.Default;
        public Vector2[] SnapablePoints { get; init; }
        public ParametricLine2[] SnapableLines { get; } = new ParametricLine2[0];

        public Point(double x, double y, double z) : this(x, y, z, Style.Default) { }
        public Point(double x, double y, double z, Style style)
        {
            X = x;
            Y = y;
            Z = z;
            Style = style;

            //Adds snapable points
            List<Vector2> snapPts = new(2);
            if (!double.IsNaN(Y))
                snapPts.Add((x, y));
            if (!double.IsNaN(Z))
                snapPts.Add((x, -z));
            SnapablePoints = snapPts.ToArray();
        }

        public void Draw(GeometryDrawer gd)
        {
            //Y point cross
            if (!double.IsNaN(Y))
            {
                gd.DrawLine((X - .2, Y), (X + .2, Y), Style);
                gd.DrawLine((X, Y - .2), (X, Y + .2), Style);
            }
            //Z point cross
            if (!double.IsNaN(Z))
            {
                gd.DrawLine((X - .2, -Z), (X + .2, -Z), Style);
                gd.DrawLine((X, -Z - .2), (X, -Z + .2), Style);
            }
        }
    }
}
