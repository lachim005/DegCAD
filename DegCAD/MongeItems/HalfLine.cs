using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using DegCAD.GeometryCommands;

namespace DegCAD.MongeItems
{
    public class HalfLine : GeometryElement, ISnapable, ISvgConvertable
    {
        private ParametricLine2 _paraLine;
        private readonly Line _line;

        public Vector2[] SnapablePoints { get; private set; }
        public Circle2[] SnapableCircles { get; } = [];
        public ParametricSegment2[] SnapableLines { get; private set; }

        public ParametricLine2 Line
        {
            get => _paraLine;
            set
            {
                _paraLine = value;
                SnapableLines[0] = new(_paraLine, 0, double.NegativeInfinity);
                SnapablePoints[0] = _paraLine.Point;
            }
        }

        public Vector2 StartPoint
        {
            get => _paraLine.Point;
            set 
            {
                _paraLine.Point = value;
                SnapablePoints[0] = value;
            }
        }
        public Vector2 Direction
        {
            get => _paraLine.DirectionVector;
            set => _paraLine.DirectionVector = value;
        }

        public HalfLine(Vector2 start, Vector2 direction, Style style, ViewportLayer? vpl = null)
        {
            ParametricLine2 line = new(start, direction);
            _paraLine = line;
            SnapableLines = [new(_paraLine, 0, double.NegativeInfinity)];
            SnapablePoints = [start];
            Style = style;

            _line = new();
            AddShape(_line);

            if (vpl is not null) AddToViewportLayer(vpl);
        }    

        public override void Draw()
        {
            if (ViewportLayer is null) return;
            _line.SetParaLine(ViewportLayer, Line, 0, double.PositiveInfinity);
        }

        public override GeometryElement CloneElement() => new HalfLine(StartPoint, Direction, Style);
        public string ToSvg() => $"<path d=\"M {_line.X1} {_line.Y1} L {_line.X2} {_line.Y2}\" {Style.ToSvgParameters()}/>";
    }
}
