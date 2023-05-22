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


        public Vector2[] SnapablePoints { get; init; }

        public ParametricLine2[] SnapableLines { get; init; }

        public Circle2[] SnapableCircles { get; init; }

        public Circle2 Circle { get; init; }
        public Vector2 Center => Circle.Center;
        public double StartAngle { get; init; }
        public double EndAngle { get; init; }
        public Style Style { 
            get => _style; 
            set
            {
                _style = value;
                _arc.SetStyle(value);
            }
        }

        public Arc(Vector2 center, Vector2 point, double startPoint, double endPoint, Style style, ViewportLayer? vpl = null)
            : this(new Circle2(center, point), startPoint, endPoint, style, vpl) { }

        public Arc(Circle2 circle, double startPoint, double endPoint, Style style, ViewportLayer? vpl = null)
        {
            Circle = circle;
            StartAngle = startPoint;
            EndAngle = endPoint;
            Style = style;

            SnapablePoints = new Vector2[3] {
                Circle.Center,
                circle.CalculatePointWithAngle(startPoint),
                circle.CalculatePointWithAngle(endPoint)
            };
            SnapableLines = new ParametricLine2[0];
            SnapableCircles = new Circle2[1] { circle };

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        Path _arc = new();

        public void Draw(ViewportLayer vpl)
        {
            _arc.SetArc(vpl, Circle, StartAngle, EndAngle);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_arc);
        }

        public void SetVisibility(Visibility visibility)
        {
            _arc.Visibility = visibility;
        }

        public IMongeItem Clone() => new Arc(Circle, StartAngle, EndAngle, Style);
    }
}
