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
    public class LineProjection : IMongeItem
    {
        private Style _style;
        private ViewportLayer? _vpl;
        private ParametricLine2 _paraLine;
        private bool _plane;

        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public ParametricLine2[] SnapableLines { get; private set; }

        public ParametricLine2 Line
        {
            get => _paraLine;
            set
            {
                _paraLine = value;
                SnapableLines[0] = _paraLine;
                RecalculateInfinitySign();
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

        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _line.SetStyle(value);
            }
        }

        public LineProjection(ParametricLine2 line, bool plane, Style style, ViewportLayer? vpl = null)
        {
            _paraLine = line;
            _plane = plane;

            RecalculateInfinitySign();
            SnapableLines = new ParametricLine2[1] { line };


            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }


        private readonly Line _line = new();

        public void Draw()
        {
            if (_vpl is null) return;
            if (Line.DirectionVector.Y == 0)
                _line.SetParaLine(_vpl, Line, double.NegativeInfinity, double.PositiveInfinity);
            else
                _line.SetParaLine(_vpl, Line, Line.GetParamFromY(0), double.PositiveInfinity * infinitySign);
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

        public IMongeItem Clone() => new LineProjection(Line, Plane, Style);

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
