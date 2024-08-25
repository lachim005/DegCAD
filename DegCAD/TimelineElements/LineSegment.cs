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

namespace DegCAD.TimelineElements
{
    public class LineSegment : GeometryElement, ISnapable
    {
        private Vector2 _p1;
        private Vector2 _p2;
        private readonly Line _line;

        public Vector2 P1
        {
            get => _p1;
            set
            {
                _p1 = value;
                SnapablePoints[0] = _p1;
                SnapableLines[0] = new(ParametricLine2.From2Points(_p1, _p2), 0, 1);
            }
        }
        public Vector2 P2
        {
            get => _p2;
            set
            {
                _p2 = value;
                SnapablePoints[1] = _p2;
                SnapableLines[0] = new(ParametricLine2.From2Points(_p1, _p2), 0, 1);
            }
        }

        public Vector2[] SnapablePoints { get; private set; }
        public ParametricSegment2[] SnapableLines { get; private set; }
        public Circle2[] SnapableCircles { get; } = [];

        public LineSegment(Vector2 p1, Vector2 p2, ViewportLayer? vpl = null) : this(p1, p2, Style.Default, vpl) { }
        public LineSegment(Vector2 p1, Vector2 p2, Style style, ViewportLayer? vpl = null)
        {
            SnapableLines = [new(ParametricLine2.From2Points(p1, p2), 0, 1)];
            SnapablePoints = [p1, p2];

            _p1 = p1;
            _p2 = p2;

            _line = new();
            AddShape(_line);

            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }


        public override void Draw()
        {
            if (ViewportLayer is null) return;
            _line.SetLineSegment(ViewportLayer, P1, P2);
        }

        public override GeometryElement CloneElement() => new LineSegment(P1, P2, Style);
    }
}
