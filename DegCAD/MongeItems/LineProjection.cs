using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace DegCAD.MongeItems
{
    /// <summary>
    /// Projection of a line in one plane
    /// </summary>
    public class LineProjection : GeometryElement, ISnapable, ISvgConvertable
    {
        private ParametricLine2 _paraLine;
        private bool _plane;
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
                RecalculateInfinitySign();
                if (Line.DirectionVector.Y == 0)
                    SnapableLines[0] = new(_paraLine, double.NegativeInfinity, double.PositiveInfinity);
                else
                    SnapableLines[0] = new(_paraLine, Line.GetParamFromY(0), double.NegativeInfinity * infinitySign);
            }
        }
        private int infinitySign;

        public bool Plane
        {
            get => _plane;
            set
            {
                _plane = value;
                RecalculateInfinitySign();
            }
        }

        public LineProjection(ParametricLine2 line, bool plane, Style style, ViewportLayer? vpl = null)
        {
            _paraLine = line;
            _plane = plane;

            RecalculateInfinitySign();
            if (Line.DirectionVector.Y == 0)
                SnapableLines = [new(_paraLine, double.NegativeInfinity, double.PositiveInfinity)];
            else
                SnapableLines = [new(_paraLine, Line.GetParamFromY(0), double.NegativeInfinity * infinitySign)];

            _line = new();
            AddShape(_line);

            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        public override void Draw()
        {
            if (ViewportLayer is null) return;
            if (Line.DirectionVector.Y == 0)
                _line.SetParaLine(ViewportLayer, Line, double.NegativeInfinity, double.PositiveInfinity);
            else
                _line.SetParaLine(ViewportLayer, Line, Line.GetParamFromY(0), double.PositiveInfinity * infinitySign);
        }

        public override GeometryElement CloneElement() => new LineProjection(Line, Plane, Style);
        public string ToSvg() => $"<path d=\"M {_line.X1} {_line.Y1} L {_line.X2} {_line.Y2}\" {Style.ToSvgParameters()}/>";

        private void RecalculateInfinitySign()
        {
            //Calculates the infinity sign for drawing the line
            infinitySign = 1;
            if (Line.DirectionVector.Y > 0)
            {
                infinitySign = -1;
            }

            if (_plane)
                infinitySign *= -1;
        }
    }
}
