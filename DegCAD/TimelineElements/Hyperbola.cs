using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DegCAD.TimelineElements
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
                RecalculateHyperbola();
            }
        }
        public Vector2 Vertex
        {
            get => _vertex;
            set
            {
                _vertex = value;
                RecalculateHyperbola();
            }
        }
        public Vector2 Point
        {
            get => _point;
            set
            {
                _point = value;
                RecalculateHyperbola();
            }
        }

        public double Eccentricity { get; set; }
        public double A { get; set; }
        public Vector2 Focus1 { get; set; }
        public Vector2 Focus2 { get; set; }
        public List<Vector2> HyperbolaPoints { get; set; } = [];
        public List<Vector2> ControlPoints { get; set; } = [];


        public Hyperbola(Vector2 vertex, Vector2 center, Vector2 point, Style style, ViewportLayer? vpl = null)
        {
            _center = center;
            _vertex = vertex;
            _point = point;
            RecalculateHyperbola();

            _hyperbola = new();
            AddShape(_hyperbola);

            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        private void RecalculateHyperbola()
        {
            var endFromOrigin = _point - _center;
            var er = endFromOrigin.RotateVector(-(_vertex - _center).Angle);
            A = (_vertex - _center).Length;
            var a2 = A * A;
            var x2 = er.X * er.X;
            var y2 = er.Y * er.Y;
            Eccentricity = Math.Sqrt(a2 * (x2 + y2 - a2) / (x2 - a2));

            Focus1 = _center + (_vertex - _center).ChangeLength(Eccentricity);
            Focus2 = _center - (_vertex - _center).ChangeLength(Eccentricity);

            List<Vector2> points1 = [];
            List<Vector2> points2 = [];
            var arad = Eccentricity - A - .6;
            var end = (Point - Focus1).Length;

            bool final = false;
            while (arad < end)
            {
                arad += 1;

                if (arad > end && !final)
                {
                    arad = end;
                    end++;
                    final = true;
                }

                Circle2 c1 = new(Focus1, arad);
                Circle2 c2 = new(Focus2, arad + 2 * A);
                var pts = c1.FindIntersections(c2);
                if (pts is null) continue;
                points1.Add(pts.Value.Item1);
                points2.Add(pts.Value.Item2);
            }

            points1.Reverse();

            HyperbolaPoints.Clear();
            HyperbolaPoints.AddRange(points1);
            HyperbolaPoints.Add(_vertex);
            HyperbolaPoints.AddRange(points2);

            if (HyperbolaPoints.Count < 4) return;

            ControlPoints.Clear();
            ControlPoints.AddRange(CRSpline.CalculateControlPoints(HyperbolaPoints));
        }

        public override void Draw()
        {
            if (ViewportLayer is null) return;
            if (HyperbolaPoints.Count < 4) return;

            _hyperbola.SetCatmullRomSpline(ViewportLayer, HyperbolaPoints, ControlPoints);
        }

        public override GeometryElement CloneElement() => new Hyperbola(Vertex, Center, Point, Style);
        public string ToSvg() => $"<path d=\"{_hyperbola.Data}\" {Style.ToSvgParameters()}/>";
    }
}
