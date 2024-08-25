using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DegCAD.TimelineElements
{
    /// <summary>
    /// Circle defined by it's center and a point on it
    /// </summary>
    public class Circle : GeometryElement, ISnapable
    {
        private Circle2 _circle2;
        private readonly System.Windows.Shapes.Ellipse _circle;

        public Vector2[] SnapablePoints { get; init; }
        public ParametricSegment2[] SnapableLines { get; } = [];
        public Circle2[] SnapableCircles { get; init; }
        public Circle2 Circle2
        {
            get => _circle2;
            set
            {
                _circle2 = value;
                SnapableCircles[0] = _circle2;
                SnapablePoints[0] = _circle2.Center;
            }
        }

        public Circle(Circle2 circle, Style style, ViewportLayer? vpl = null)
        {
            Style = style;
            _circle2 = circle;

            SnapablePoints = [circle.Center];
            SnapableCircles = [circle];

            _circle = new();
            AddShape(_circle);

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        public override void Draw()
        {
            if (ViewportLayer is null) return;
            _circle.SetCircle(ViewportLayer, Circle2);
        }

        public override GeometryElement CloneElement() => new Circle(Circle2, Style);
    }
}
