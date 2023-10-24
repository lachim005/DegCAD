using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DegCAD.MongeItems
{
    public class Plane : IMongeItem
    {
        private Style _style;
        private ViewportLayer? _vpl;

        public bool TopPlane { get; set; }

        public Vector2[] SnapablePoints => new Vector2[0];

        public ParametricSegment2[] SnapableLines => new ParametricSegment2[0];

        public Circle2[] SnapableCircles => new Circle2[0];

        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _rectangle.Fill = new SolidColorBrush(_style.Color);
            }
        }

        public Plane(bool plane, ViewportLayer? vpl = null) : this(plane, new() { Color = Color.FromRgb(255, 255, 200) }, vpl) { }
        public Plane(bool plane, Style style, ViewportLayer? vpl = null)
        {
            TopPlane = plane;

            Style = style;
            if (vpl is not null)
                AddToViewportLayer(vpl);
        }

        Rectangle _rectangle = new();

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_rectangle);
            _vpl = vpl;
        }

        public void RemoveFromViewportLayer()
        {
            if (_vpl is null) return;
            _vpl.Canvas.Children.Remove(_rectangle);
            _vpl = null;
        }

        public IMongeItem Clone() => new Plane(TopPlane);


        public void Draw()
        {
            if (_vpl is null) return;
            _rectangle.Width = _vpl.Canvas.ActualWidth;
            var split = _vpl.Viewport.CanvasToScreen((0, 0));
            if (TopPlane)
            {
                Canvas.SetTop(_rectangle, 0);
                if (split.Y < 0)
                {
                    _rectangle.Height = 0;
                    return;
                }
                if (split.Y > _vpl.Canvas.ActualHeight)
                {
                    _rectangle.Height = _vpl.Canvas.ActualHeight;
                    return;
                }
                _rectangle.Height = split.Y;
                return;
            }
            if (split.Y < 0)
            {
                _rectangle.Height = _vpl.Canvas.ActualHeight;
                Canvas.SetTop(_rectangle, 0);
                return;
            }
            if (split.Y > _vpl.Canvas.ActualHeight)
            {
                _rectangle.Height = 0;
                return;
            }
            Canvas.SetTop( _rectangle, split.Y);
            _rectangle.Height = _vpl.Canvas.ActualHeight - split.Y;
        }

        public void SetVisibility(Visibility visibility)
        {
            _rectangle.Visibility = visibility;
        }
    }
}
