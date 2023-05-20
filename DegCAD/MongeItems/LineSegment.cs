using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DegCAD.MongeItems
{
    public class LineSegment : IMongeItem
    {
        private Style _style;

        public Vector2 P1 { get; init; }
        public Vector2 P2 { get; init; }
        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _line.SetStyle(value);
            }
        }

        public Vector2[] SnapablePoints { get; init; }
        public ParametricLine2[] SnapableLines { get; init; }
        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public LineSegment(Vector2 p1, Vector2 p2, ViewportLayer? vpl = null) : this(p1, p2, Style.Default, vpl) { }
        public LineSegment(Vector2 p1, Vector2 p2, Style style, ViewportLayer? vpl = null)
        {
            P1 = p1;
            P2 = p2;
            Style = style;
            SnapableLines = new ParametricLine2[1] { ParametricLine2.From2Points(p1, p2) };
            SnapablePoints = new Vector2[2] { p1, p2 };

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        private readonly Line _line = new();

        public void Draw(ViewportLayer vpl)
        {
            _line.SetLineSegment(vpl, P1, P2);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_line);
        }

        public void SetVisibility(Visibility visibility)
        {
            _line.Visibility = visibility;
        }
    }
}
