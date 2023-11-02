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
    public class Parabola : IMongeItem
    {
        private Style _style;
        private ViewportLayer? _vpl;
        private Vector2 _focus;
        private Vector2 _vertex;
        private Vector2 _end;

        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public ParametricSegment2[] SnapableLines { get; } = new ParametricSegment2[0];

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _parabola.SetStyle(value);
            }
        }
        public Vector2 Focus
        {
            get => _focus;
            set
            {
                _focus = value;
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
        public Vector2 End
        {
            get => _end;
            set
            {
                _end = value;
                RecalculateSvgPoints();
            }
        }
        public bool Infinite { get; set; } = true;

        public Vector2 EndPoint1 { get; private set; }
        public Vector2 EndPoint2 { get; private set; }
        public Vector2 ControlPoint { get; private set; }

        public Parabola(Vector2 focus, Vector2 vertex, Vector2 end, Style style, ViewportLayer? vpl = null)
            : this(focus, vertex, style, vpl)
        {
            End = end;
            Infinite = false;
        }

        public Parabola(Vector2 focus, Vector2 vertex, Style style, ViewportLayer? vpl = null)
        {
            Focus = focus;
            Vertex = vertex;

            Style = style;

            _parabola.RenderTransform = new RotateTransform();

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        System.Windows.Shapes.Path _parabola = new();

        private void RecalculateSvgPoints()
        {
            Vector2 v1 = Vertex - Focus;
            ParametricLine2 controlLine = new(Vertex + v1, (v1.Y, -v1.X));
            double rad = (End - controlLine.GetClosestPoint(End)).Length;
            Circle2 c1 = new(Focus, rad);
            ParametricLine2 endLine = new(End, controlLine.DirectionVector);
            var iPts = c1.FindIntersections(endLine);
            if (iPts is null)
            {
                EndPoint1 = Vertex;
                EndPoint2 = Vertex;
                ControlPoint = Vertex;
                return;
            }

            EndPoint1 = iPts.Value.Item1;
            EndPoint2 = iPts.Value.Item2;

            var q = controlLine.GetClosestPoint(EndPoint1);
            ParametricLine2 p = new(q, Focus - q);
            ParametricLine2 vertexLine = new(Vertex, controlLine.DirectionVector);
            var P = p.FindIntersection(vertexLine);
            ParametricLine2 t = new(P, EndPoint1 - P);
            ParametricLine2 axis = new(Vertex, v1);
            ControlPoint = axis.FindIntersection(t);
        }

        public void Draw()
        {
            if (_vpl is null) return;
            if (!Infinite)
                _parabola.SetParabola(_vpl, this);
            else
                _parabola.SetInfiniteParabola(_vpl, this);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            _vpl = vpl;
            vpl.Canvas.Children.Add(_parabola);
        }
        public void RemoveFromViewportLayer()
        {
            if (_vpl is null) return;
            _vpl.Canvas.Children.Remove(_parabola);
            _vpl = null;
        }
        public void SetVisibility(Visibility visibility)
        {
            _parabola.Visibility = visibility;
        }
        public bool IsVisible() => _parabola.Visibility == Visibility.Visible;
        public IMongeItem Clone() => new Parabola(Focus, Vertex, End, Style);
        public string ToSvg() => $"<path d=\"{_parabola.Data}\" {Style.ToSvgParameters()}/>";
    }
}
