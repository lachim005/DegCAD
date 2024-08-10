using DegCAD.GeometryCommands;
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
    public class Plane : GeometryElement
    {
        private readonly Rectangle _rectangle;

        public bool TopPlane { get; set; }

        public Plane(bool plane, ViewportLayer? vpl = null) : this(plane, new() { Color = Color.FromArgb(55, 255, 255, 0) }, vpl) { }
        public Plane(bool plane, Style style, ViewportLayer? vpl = null)
        {
            TopPlane = plane;

            _rectangle = new();
            AddShape(_rectangle);

            Style = style;
            if (vpl is not null)
                AddToViewportLayer(vpl);
        }

        public override GeometryElement CloneElement() => new Plane(TopPlane);

        public override void ShowStyle(Style style)
        {
            _rectangle.Fill = new SolidColorBrush(Style.Color);
        }

        public override void Draw()
        {
            if (ViewportLayer is null) return;
            _rectangle.Width = ViewportLayer.Canvas.ActualWidth;
            var split = ViewportLayer.Viewport.CanvasToScreen((0, 0));
            if (TopPlane)
            {
                Canvas.SetTop(_rectangle, 0);
                if (split.Y < 0)
                {
                    _rectangle.Height = 0;
                    return;
                }
                if (split.Y > ViewportLayer.Canvas.ActualHeight)
                {
                    _rectangle.Height = ViewportLayer.Canvas.ActualHeight;
                    return;
                }
                _rectangle.Height = split.Y;
                return;
            }
            if (split.Y < 0)
            {
                _rectangle.Height = ViewportLayer.Canvas.ActualHeight;
                Canvas.SetTop(_rectangle, 0);
                return;
            }
            if (split.Y > ViewportLayer.Canvas.ActualHeight)
            {
                _rectangle.Height = 0;
                return;
            }
            Canvas.SetTop( _rectangle, split.Y);
            _rectangle.Height = ViewportLayer.Canvas.ActualHeight - split.Y;
        }
    }
}
