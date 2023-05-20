using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Windows.Shapes;

namespace DegCAD.DrawableItems
{
    internal class Point : IMongeItem
    {
        private Style _style;

        public double X { get; init; }
        public double Y { get; init; }
        public Vector2 Coords => new Vector2(X, Y);
        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _line1.SetStyle(value);
                _line2.SetStyle(value);
            }
        }
        public Vector2[] SnapablePoints { get; init; }
        public ParametricLine2[] SnapableLines { get; } = new ParametricLine2[0];
        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Point(double x, double y, ViewportLayer? vpl = null) : this(x, y, Style.Default, vpl) { }
        public Point(double x, double y, Style style, ViewportLayer? vpl = null)
        {
            style.LineStyle = 0;

            X = x;
            Y = y;
            Style = style;

            //Adds snapable points
            SnapablePoints = new Vector2[1] { new(X, Y) };

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        private readonly Line _line1 = new();
        private readonly Line _line2 = new();

        public void Draw(ViewportLayer vpl)
        {
            Draw(vpl, Style);
        }
        public void Draw(ViewportLayer vpl, Style s)
        {
            _line1.SetLineSegment(vpl, (Coords.X, Coords.Y + .2), (Coords.X, Coords.Y - .2));
            _line2.SetLineSegment(vpl, (Coords.X + .2, Coords.Y), (Coords.X - .2, Coords.Y));
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_line1);
            vpl.Canvas.Children.Add(_line2);
        }
    }
}
