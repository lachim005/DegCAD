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

        public Vector2[] SnapablePoints { get; init; }
        public ParametricLine2[] SnapableLines { get; init; }
        public Circle2[] SnapableCircles { get; init; }
        public Circle2 Circle2 { get; init; }
        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _circle.SetStyle(value);
            }
        }

        public Circle(Vector2 center, Vector2 pointOnCircle, Style style, ViewportLayer? vpl = null)
            : this(new Circle2(center, pointOnCircle), style, vpl)
        {

        }

        public Circle(Circle2 circle, Style style, ViewportLayer? vpl = null)
        {
            Style = style;
            Circle2 = circle;

            SnapableLines = new ParametricLine2[0];
            SnapablePoints = new Vector2[1] { circle.Center };
            SnapableCircles = new Circle2[1] { circle };

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        Ellipse _circle = new();

        public void Draw(ViewportLayer vpl)
        {
            _circle.SetCircle(vpl, Circle2);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_circle);
        }

        public void SetVisibility(Visibility visibility)
        {
            _circle.Visibility = visibility;
        }
    }
}
