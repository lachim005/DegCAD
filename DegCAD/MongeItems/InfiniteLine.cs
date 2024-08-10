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
    internal class InfiniteLine : GeometryElement, ISnapable, ISvgConvertable
    {
        private ParametricLine2 _paraLine;
        private readonly Line _line;

        public Vector2[] SnapablePoints { get; } = [];

        public Circle2[] SnapableCircles { get; } = [];

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

        public InfiniteLine(ParametricLine2 line, Style style, ViewportLayer? vpl = null)
        {
            _paraLine = line;
            SnapableLines = [new(_paraLine, double.NegativeInfinity, double.PositiveInfinity)];

            _line = new();
            AddShape(_line);

            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        } 

        public override void Draw()
        {
            if (ViewportLayer is null) return;
            _line.SetParaLine(ViewportLayer, Line, double.NegativeInfinity, double.PositiveInfinity);
        }

        public override GeometryElement CloneElement() => new InfiniteLine(Line, Style);
        public string ToSvg() => $"<path d=\"M {_line.X1} {_line.Y1} L {_line.X2} {_line.Y2}\" {Style.ToSvgParameters()}/>";
    }
}
