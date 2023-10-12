﻿using System;
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

        public ParametricLine2[] SnapableLines { get; private set; }

        public ParametricLine2 Line
        {
            get => _paraLine;
            set
            {
                _paraLine = value;
                SnapableLines[0] = _paraLine;
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

        public InfiniteLine(ParametricLine2 line, Style style, ViewportLayer? vpl = null)
        {
            SnapableLines = new ParametricLine2[1] { line };
            _paraLine = line;

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

        public IMongeItem Clone() => new InfiniteLine(Line, Style);
    }
}
