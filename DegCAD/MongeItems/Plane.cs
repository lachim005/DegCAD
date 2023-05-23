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
        private ViewportLayer _vpl;

        public bool TopPlane { get; set; }

        public Vector2[] SnapablePoints => new Vector2[0];

        public ParametricLine2[] SnapableLines => new ParametricLine2[0];

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

        public Plane(bool plane, ViewportLayer vpl) : this(plane, new() { Color = Color.FromRgb(255, 255, 200) }, vpl) { }
        public Plane(bool plane, Style style, ViewportLayer vpl)
        {
            _vpl = vpl;
            TopPlane = plane;

            Style = style;
            AddToViewportLayer(vpl);
        }

        Rectangle _rectangle = new();

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_rectangle);
        }

        public IMongeItem Clone() => new Plane(TopPlane, _vpl);


        public void Draw(ViewportLayer vpl)
        {
            _rectangle.Width = vpl.Canvas.ActualWidth;
            var split = vpl.Viewport.CanvasToScreen((0, 0));
            if (TopPlane)
            {
                Canvas.SetTop(_rectangle, 0);
                if (split.Y < 0)
                {
                    _rectangle.Height = 0;
                    return;
                }
                if (split.Y > vpl.Canvas.ActualHeight)
                {
                    _rectangle.Height = vpl.Canvas.ActualHeight;
                    return;
                }
                _rectangle.Height = split.Y;
                return;
            }
            if (split.Y < 0)
            {
                _rectangle.Height = vpl.Canvas.ActualHeight;
                Canvas.SetTop(_rectangle, 0);
                return;
            }
            if (split.Y > vpl.Canvas.ActualHeight)
            {
                _rectangle.Height = 0;
                return;
            }
            Canvas.SetTop( _rectangle, split.Y);
            _rectangle.Height = vpl.Canvas.ActualHeight - split.Y;
        }

        public void SetVisibility(Visibility visibility)
        {
            _rectangle.Visibility = visibility;
        }
    }
}
