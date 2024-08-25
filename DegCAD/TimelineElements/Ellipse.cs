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

namespace DegCAD.TimelineElements
{
    public class Ellipse : GeometryElement
    {
        private readonly System.Windows.Shapes.Ellipse _ellipse;
        private readonly RotateTransform _ellTransform;

        public Vector2 Center { get; set; }
        public Vector2 P1 { get; set; }
        public Vector2 P2 { get; set; }

        public Ellipse(Vector2 center, Vector2 pt1, Vector2 pt2, Style style, ViewportLayer? vpl = null)
        {
            Center = center;
            P1 = pt1;
            P2 = pt2;

            Style = style;

            _ellipse = new();
            _ellTransform = new RotateTransform();
            _ellipse.RenderTransform = _ellTransform;
            AddShape(_ellipse);

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        public override void Draw()
        {
            if (ViewportLayer is null) return;
            _ellipse.SetEllipse(ViewportLayer, Center, P1, P2);
        }

        public override GeometryElement CloneElement() => new Ellipse(Center, P1, P2, Style);
    }
}
