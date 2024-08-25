using DegCAD.GeometryCommands;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace DegCAD.TimelineElements
{
    internal class Point : GeometryElement, ISnapable
    {
        private double _x;
        private double _y;
        private readonly Line _line1;
        private readonly Line _line2;

        public double X 
        { 
            get => _x;
            set
            {
                _x = value;
                SnapablePoints[0].X = _x;
            }
        }
        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                SnapablePoints[0].Y = _y;
            }
        }
        public Vector2 Coords
        {
            set
            {
                X = value.X;
                Y = value.Y;
            }
            get
            {
                return (X, Y);
            }
        }

        public Vector2[] SnapablePoints { get; private set; }
        public ParametricSegment2[] SnapableLines { get; } = [];
        public Circle2[] SnapableCircles { get; } = [];

        public Point(double x, double y, ViewportLayer? vpl = null) : this(x, y, Style.Default, vpl) { }
        public Point(double x, double y, Style style, ViewportLayer? vpl = null)
        {
            style.LineStyle = 0;

            //Adds snapable points
            SnapablePoints = new Vector2[1] { new(x, y) };

            _line1 = new();
            _line2 = new();
            AddShape(_line1);
            AddShape(_line2);

            _x = x;
            _y = y;
            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        public override void Draw()
        {
            if (ViewportLayer is null) return;
            _line1.SetLineSegment(ViewportLayer, (Coords.X, Coords.Y + .2), (Coords.X, Coords.Y - .2));
            _line2.SetLineSegment(ViewportLayer, (Coords.X + .2, Coords.Y), (Coords.X - .2, Coords.Y));
        }

        public override GeometryElement CloneElement() => new Point(X, Y, Style);
    }
}
