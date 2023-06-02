using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace DegCAD.MongeItems
{
    /// <summary>
    /// Circle defined by it's center and a point on it
    /// </summary>
    public class Circle : IMongeItem
    {
        private Style _style;
        private ViewportLayer? _vpl;
        private Circle2 _circle2;

        public Vector2[] SnapablePoints { get; init; }
        public ParametricLine2[] SnapableLines { get; } = new ParametricLine2[0];
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
        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _circle.SetStyle(value);
            }
        }

        public Circle(Circle2 circle, Style style, ViewportLayer? vpl = null)
        {
            Style = style;
            _circle2 = circle;

            SnapablePoints = new Vector2[1] { circle.Center };
            SnapableCircles = new Circle2[1] { circle };

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        System.Windows.Shapes.Ellipse _circle = new();

        public void Draw()
        {
            if (_vpl is null) return;
            _circle.SetCircle(_vpl, Circle2);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_circle);
            _vpl = vpl;
        }
        public void RemoveFromViewportLayer()
        {
            if (_vpl is null) return;
            _vpl.Canvas.Children.Remove(_circle);
            _vpl = null;
        }
        public void SetVisibility(Visibility visibility)
        {
            _circle.Visibility = visibility;
        }

        public IMongeItem Clone() => new Circle(Circle2, Style);
    }
}
