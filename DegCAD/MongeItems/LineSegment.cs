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
        private ViewportLayer? _vpl;
        private Vector2 _p1;
        private Vector2 _p2;

        public Vector2 P1
        {
            get => _p1;
            set
            {
                _p1 = value;
                SnapablePoints[0] = _p1;
                SnapableLines[0] = ParametricLine2.From2Points(_p1, _p2);
            }
        }
        public Vector2 P2
        {
            get => _p2;
            set
            {
                _p2 = value;
                SnapablePoints[1] = _p2;
                SnapableLines[0] = ParametricLine2.From2Points(_p1, _p2);
            }
        }
        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _line.SetStyle(value);
            }
        }

        public Vector2[] SnapablePoints { get; private set; }
        public ParametricLine2[] SnapableLines { get; private set; }
        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public LineSegment(Vector2 p1, Vector2 p2, ViewportLayer? vpl = null) : this(p1, p2, Style.Default, vpl) { }
        public LineSegment(Vector2 p1, Vector2 p2, Style style, ViewportLayer? vpl = null)
        {
            SnapableLines = new ParametricLine2[1] { ParametricLine2.From2Points(p1, p2) };
            SnapablePoints = new Vector2[2] { p1, p2 };

            _p1 = p1;
            _p2 = p2;
            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        private readonly Line _line = new();

        public void Draw()
        {
            if (_vpl is null) return;
            _line.SetLineSegment(_vpl, P1, P2);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_line);
            _vpl = vpl;
        }
        public void RemoveFromViewportLayer()
        {
            if (_vpl is null) return;
            _vpl.Canvas.Children.Remove(_line);
            _vpl = null;
        }

        public void SetVisibility(Visibility visibility)
        {
            _line.Visibility = visibility;
        }

        public IMongeItem Clone() => new LineSegment(P1, P2, Style);
    }
}
