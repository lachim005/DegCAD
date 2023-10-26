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
    public class Arc : IMongeItem
    {
        private Style _style;
        private ViewportLayer? _vpl;
        private Circle2 _circle2;
        private double _startAngle;
        private double _endAngle;


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
        public Style Style { 
            get => _style; 
            set
            {
                _style = value;
                _arc.SetStyle(value);
            }
        }

        public Arc(Circle2 circle, double startPoint, double endPoint, Style style, ViewportLayer? vpl = null)
        {
            SnapablePoints = new Vector2[3] {
                circle.Center,
                circle.CalculatePointWithAngle(startPoint),
                circle.CalculatePointWithAngle(endPoint)
            };
            SnapableLines = new ParametricSegment2[0];
            SnapableCircles = new Circle2[1] { circle };


            _circle2 = circle;
            _startAngle = startPoint;
            _endAngle = endPoint;

            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        Path _arc = new();

        public void Draw()
        {
            if (_vpl is null) return;
            _arc.SetArc(_vpl, Circle, StartAngle, EndAngle);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_arc);
            _vpl = vpl;
        }
        public void RemoveFromViewportLayer()
        {
            if (_vpl is null) return;
            _vpl.Canvas.Children.Remove(_arc);
            _vpl = null;
        }

        public void SetVisibility(Visibility visibility)
        {
            _arc.Visibility = visibility;
        }

        public bool IsVisible() => _arc.Visibility == Visibility.Visible;

        public IMongeItem Clone() => new Arc(Circle, StartAngle, EndAngle, Style);
    }
}
