using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DegCAD.MongeItems
{
    public class Ellipse : IMongeItem
    {
        private Style _style;
        private ViewportLayer? _vpl;

        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public ParametricSegment2[] SnapableLines { get; } = new ParametricSegment2[0];

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _ellipse.SetStyle(value);
            }
        }
        public Vector2 Center { get; set; }
        public Vector2 P1 { get; set; }
        public Vector2 P2 { get; set; }

        public Ellipse(Vector2 center, Vector2 pt1, Vector2 pt2, Style style, ViewportLayer? vpl = null)
        {
            Center = center;
            P1 = pt1;
            P2 = pt2;

            Style = style;

            _ellTransform = new RotateTransform();
            _ellipse.RenderTransform = _ellTransform;

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        System.Windows.Shapes.Ellipse _ellipse = new();
        RotateTransform _ellTransform;

        public void Draw()
        {
            if (_vpl is null) return;
            _ellipse.SetEllipse(_vpl, Center, P1, P2);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            _vpl = vpl;
            vpl.Canvas.Children.Add(_ellipse);
        }
        public void RemoveFromViewportLayer()
        {
            if (_vpl is null) return;
            _vpl.Canvas.Children.Remove(_ellipse);
            _vpl = null;
        }
        public void SetVisibility(Visibility visibility)
        {
            _ellipse.Visibility = visibility;
        }
        public bool IsVisible() => _ellipse.Visibility == Visibility.Visible;

        public IMongeItem Clone() => new Ellipse(Center, P1, P2, Style);
        public string ToSvg()
        {
            double x = Canvas.GetLeft(_ellipse) + _ellipse.Width / 2;
            double y = Canvas.GetTop(_ellipse) + _ellipse.Height / 2;
            return $"<ellipse " +
            $"cx=\"{x}\" " +
            $"cy=\"{y}\" " +
            $"rx=\"{_ellipse.Width / 2}\" " +
            $"ry=\"{_ellipse.Height / 2}\" " +
            $"transform=\"rotate({_ellTransform.Angle} {x} {y})\" " +
            $"{Style.ToSvgParameters()}/>";
        }
    }
}
