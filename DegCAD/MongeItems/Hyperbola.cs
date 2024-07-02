using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DegCAD.MongeItems
{
    public class Hyperbola : IMongeItem
    {
        private Style _style;
        private ViewportLayer? _vpl;
        private Vector2 _center;
        private Vector2 _vertex;
        private Vector2 _point;

        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public ParametricSegment2[] SnapableLines { get; } = new ParametricSegment2[0];

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _hyperbola.SetStyle(value);
            }
        }
        public Vector2 Center
        {
            get => _center;
            set
            {
                _center = value;
                RecalculateSvgPoints();
            }
        }
        public Vector2 Vertex
        {
            get => _vertex;
            set
            {
                _vertex = value;
                RecalculateSvgPoints();
            }
        }
        public Vector2 Point
        {
            get => _point;
            set
            {
                _point = value;
                RecalculateSvgPoints();
            }
        }

        public Vector2 EndPoint1 => Point;
        public Vector2 EndPoint2 { get; private set; }
        public Vector2 ControlPoint1 { get; private set; }
        public Vector2 ControlPoint2 { get; private set; }

        public Hyperbola(Vector2 vertex, Vector2 center, Vector2 point, Style style, ViewportLayer? vpl = null)
        {
            Center = center;
            Vertex = vertex;
            Point = point;

            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        System.Windows.Shapes.Path _hyperbola = new() { IsHitTestVisible = false };

        private void RecalculateSvgPoints()
        {
            var axisDV = Vertex - Center;
            var axisPerpDV = new Vector2(-axisDV.Y, axisDV.X);
            var axis = new ParametricLine2(Vertex, axisDV);
            var endLine = new ParametricLine2(Point, axisPerpDV);

            var endMid = axis.FindIntersection(endLine);
            var endVec = endMid - Point;
            EndPoint2 = endMid + endVec;

            ControlPoint1 = Vertex - (endVec / 3);
            ControlPoint2 = Vertex + (endVec / 3);
        }

        public void Draw()
        {
            if (_vpl is null) return;
            _hyperbola.SetHyperbola(_vpl, this);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            _vpl = vpl;
            vpl.Canvas.Children.Add(_hyperbola);
        }
        public void RemoveFromViewportLayer()
        {
            if (_vpl is null) return;
            _vpl.Canvas.Children.Remove(_hyperbola);
            _vpl = null;
        }
        public void SetVisibility(Visibility visibility)
        {
            _hyperbola.Visibility = visibility;
        }
        public bool IsVisible() => _hyperbola.Visibility == Visibility.Visible;
        public IMongeItem Clone() => new Hyperbola(Vertex, Center, Point, Style);
        public string ToSvg() => $"<path d=\"{_hyperbola.Data}\" {Style.ToSvgParameters()}/>";
    }
}
