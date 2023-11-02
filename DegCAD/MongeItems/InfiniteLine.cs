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
    internal class InfiniteLine : IMongeItem
    {
        private Style _style;
        private ViewportLayer? _vpl;
        private ParametricLine2 _paraLine;

        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public ParametricSegment2[] SnapableLines { get; private set; }

        public ParametricLine2 Line
        {
            get => _paraLine;
            set
            {
                _paraLine = value;
                SnapableLines[0] = new(_paraLine, double.NegativeInfinity, double.PositiveInfinity);
            }
        }
        public Vector2 StartPoint
        {
            get => _paraLine.Point;
            set => _paraLine.Point = value;
        }
        public Vector2 Direction
        {
            get => _paraLine.DirectionVector;
            set => _paraLine.DirectionVector = value;
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

        public InfiniteLine(ParametricLine2 line, Style style, ViewportLayer? vpl = null)
        {
            _paraLine = line;
            SnapableLines = new ParametricSegment2[1] { new(_paraLine, double.NegativeInfinity, double.PositiveInfinity) };

            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }


        private readonly Line _line = new();

        public void Draw()
        {
            if (_vpl is null) return;
            _line.SetParaLine(_vpl, Line, double.NegativeInfinity, double.PositiveInfinity);
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
        public bool IsVisible() => _line.Visibility == Visibility.Visible;
        public IMongeItem Clone() => new InfiniteLine(Line, Style);
        public string ToSvg() => $"<path d=\"M {_line.X1} {_line.Y1} L {_line.X2} {_line.Y2}\" {Style.ToSvgParameters()}/>";
    }
}
