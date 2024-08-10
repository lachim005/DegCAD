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
    public class Hyperbola : GeometryElement, ISvgConvertable
    {
        private Vector2 _center;
        private Vector2 _vertex;
        private Vector2 _point;
        private readonly System.Windows.Shapes.Path _hyperbola;

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

            _hyperbola = new();
            AddShape(_hyperbola);

            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }

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

        public override void Draw()
        {
            if (ViewportLayer is null) return;
            _hyperbola.SetHyperbola(ViewportLayer, this);
        }

        public override GeometryElement CloneElement() => new Hyperbola(Vertex, Center, Point, Style);
        public string ToSvg() => $"<path d=\"{_hyperbola.Data}\" {Style.ToSvgParameters()}/>";
    }
}
