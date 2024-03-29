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
        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public ParametricSegment2[] SnapableLines { get; } = new ParametricSegment2[0];

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Style Style { get; set; } = Style.Default;

        public Axis()
        {
            
        }

        public void Draw()
        {
            
        }

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            
        }
        public void RemoveFromViewportLayer()
        {
            
        }

        public void SetVisibility(Visibility visibility)
        {
            
        }
        public bool IsVisible() => false;
        public IMongeItem Clone() => new Axis();
        public string ToSvg() => string.Empty;
    }
}
