using DegCAD.GeometryCommands;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Shapes;

namespace DegCAD.MongeItems
{
    internal class Point : IMongeItem
    {
        private Style _style;
        private ViewportLayer? _vpl;
        private double _x;
        private double _y;

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
        public Vector2[] SnapablePoints { get; private set; }
        public ParametricSegment2[] SnapableLines { get; } = new ParametricSegment2[0];
        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Point(double x, double y, ViewportLayer? vpl = null) : this(x, y, Style.Default, vpl) { }
        public Point(double x, double y, Style style, ViewportLayer? vpl = null)
        {
            style.LineStyle = 0;

            //Adds snapable points
            SnapablePoints = new Vector2[1] { new(x, y) };

            _x = x;
            _y = y;
            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        private readonly Line _line1 = new();
        private readonly Line _line2 = new();

        public void Draw()
        {
            if (_vpl is null) return;
            _line1.SetLineSegment(_vpl, (Coords.X, Coords.Y + .2), (Coords.X, Coords.Y - .2));
            _line2.SetLineSegment(_vpl, (Coords.X + .2, Coords.Y), (Coords.X - .2, Coords.Y));
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_line1);
            vpl.Canvas.Children.Add(_line2);
            _vpl = vpl;
        }
        public void RemoveFromViewportLayer()
        {
            if (_vpl is null) return;
            _vpl.Canvas.Children.Remove(_line1);
            _vpl.Canvas.Children.Remove(_line2);
            _vpl = null;
        }
        public void SetVisibility(Visibility visibility)
        {
            _line1.Visibility = visibility;
            _line2.Visibility = visibility;
        }

        public IMongeItem Clone() => new Point(X, Y, Style);
    }
}
