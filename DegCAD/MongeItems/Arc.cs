using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DegCAD.MongeItems
{
    /// <summary>
    /// An arc defined by it's center, a point on the circle and two angles
    /// </summary>
    public class Arc : GeometryElement, ISnapable, ISvgConvertable
    {
        private Circle2 _circle2;
        private double _startAngle;
        private double _endAngle;
        private readonly Path _arc;

        public Vector2[] SnapablePoints { get; init; }

        public ParametricSegment2[] SnapableLines { get; init; }

        public Circle2[] SnapableCircles { get; init; }

        public Circle2 Circle
        {
            get => _circle2;
            set
            {
                _circle2 = value;
                SnapableCircles[0] = _circle2;
                SnapablePoints[0] = _circle2.Center;
                SnapablePoints[1] = _circle2.CalculatePointWithAngle(StartAngle);
                SnapablePoints[2] = _circle2.CalculatePointWithAngle(EndAngle);
            }
        }
        public Vector2 Center => Circle.Center;
        public double StartAngle
        {
            get => _startAngle;
            set
            {
                _startAngle = value;
                SnapablePoints[1] = _circle2.CalculatePointWithAngle(_startAngle);
            }
        }
        public double EndAngle
        {
            get => _endAngle;
            set
            {
                _endAngle = value;
                SnapablePoints[2] = _circle2.CalculatePointWithAngle(_endAngle);
            }
        }

        public Arc(Circle2 circle, double startPoint, double endPoint, Style style, ViewportLayer? vpl = null)
        {
            SnapablePoints = [
                circle.Center,
                circle.CalculatePointWithAngle(startPoint),
                circle.CalculatePointWithAngle(endPoint)
            ];
            SnapableLines = [];
            SnapableCircles = [ circle ];


            _circle2 = circle;
            _startAngle = startPoint;
            _endAngle = endPoint;

            _arc = new();
            AddShape(_arc);

            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        public override void Draw()
        {
            if (ViewportLayer is null) return;
            _arc.SetArc(ViewportLayer, Circle, StartAngle, EndAngle);
        }

        public override GeometryElement CloneElement() => new Arc(Circle, StartAngle, EndAngle, Style);

        public string ToSvg() => $"<path d=\"{_arc.Data}\" {Style.ToSvgParameters()}/>";
    }
}
