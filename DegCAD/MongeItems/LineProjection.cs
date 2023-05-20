﻿using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public ParametricLine2[] SnapableLines { get; init; }

        public ParametricLine2 Line { get; init; }
        private int infinitySign;

        public bool Plane { get; init; }

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
            Line = line;
            Plane = plane;

            //Calculates the infinity sign for drawing the line
            infinitySign = 1;
            if (Line.DirectionVector.Y * Line.DirectionVector.X < 0)
            {
                infinitySign = -1;
            }

            if (plane)
                infinitySign *= -1;


            SnapableLines = new ParametricLine2[1] { line };
            Style = style;

            if (vpl is not null) AddToViewportLayer(vpl);
        }


        private readonly Line _line = new();

        public void Draw(ViewportLayer vpl)
        {
            if (Line.DirectionVector.Y == 0)
                _line.SetParaLine(vpl, Line, double.NegativeInfinity, double.PositiveInfinity);
            else
                _line.SetParaLine(vpl, Line, Line.GetParamFromY(0), double.PositiveInfinity * infinitySign);
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            vpl.Canvas.Children.Add(_line);
        }

        public void SetVisibility(Visibility visibility)
        {
            _line.Visibility = visibility;
        }
    }
}
