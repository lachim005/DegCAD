using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace DegCAD.MongeItems
{
    /// <summary>
    /// Static class that can display the x axis and the zero mark
    /// </summary>
    public class Axis : IMongeItem
    {
        private Style _style = Style.Default;

        static ParametricLine2 axis = new((0, 0), (1, 0));

        public Vector2[] SnapablePoints { get; } = new Vector2[1] { (0, 0) };

        public ParametricLine2[] SnapableLines { get; } = new ParametricLine2[1] { new((0, 0), (1, 0)) };

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Style Style
        {
            get => _style;
            set
            {
                _style = value;
                _line.SetStyle(value);
                _zeroMark.SetStyle(value);
            }
        }

        private readonly Line _line = new();
        private readonly Line _zeroMark = new();

        public Axis(Style style, ViewportLayer? vpl = null)
        {
            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }

        public void Draw(ViewportLayer vpl)
        {
            _line.SetParaLine(vpl, axis, double.NegativeInfinity, double.PositiveInfinity);
            _zeroMark.SetLineSegment(vpl, (0, .2), (0, -.2));
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_line);
            vpl.Canvas.Children.Add(_zeroMark);
        }

        public void SetVisibility(Visibility visibility)
        {
            _line.Visibility = visibility;
            _zeroMark.Visibility = visibility;
        }
    }
}
