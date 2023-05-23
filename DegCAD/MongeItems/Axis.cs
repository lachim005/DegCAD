﻿using DegCAD.GeometryCommands;
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
        private ViewportLayer? _vpl;

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

        public void Draw()
        {
            if (_vpl is null) return;
            _line.SetParaLine(_vpl, axis, double.NegativeInfinity, double.PositiveInfinity);
            _zeroMark.SetLineSegment(_vpl, (0, .2), (0, -.2));
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_line);
            vpl.Canvas.Children.Add(_zeroMark);
            _vpl = vpl;
        }
        public void RemoveFromViewportLayer()
        {
            if (_vpl is null) return;
            _vpl.Canvas.Children.Remove(_line);
            _vpl.Canvas.Children.Remove(_zeroMark);
            _vpl = null;
        }

        public void SetVisibility(Visibility visibility)
        {
            _line.Visibility = visibility;
            _zeroMark.Visibility = visibility;
        }

        public IMongeItem Clone() => new Axis(Style);
    }
}
