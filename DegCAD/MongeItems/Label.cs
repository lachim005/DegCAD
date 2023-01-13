﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MongeItems
{
    /// <summary>
    /// A label labeling a thing that's supposed to be labeled
    /// </summary>
    public class Label : IMongeItem
    {
        public string LabelText { get; set; }
        public string Subscript { get; set; }
        public string Superscript { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 Size { get; private set; }

        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public ParametricLine2[] SnapableLines { get; } = new ParametricLine2[0];

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Style Style { get; init; }

        private Action<GeometryDrawer, Style> drawLabeledObject;

        public Label(string labelText, string subscript, string superscript, Vector2 position, Style style, Action<GeometryDrawer, Style> drawLabeledObject)
        {
            LabelText = labelText;
            Subscript = subscript;
            Superscript = superscript;
            Position = position;
            Style = style;
            this.drawLabeledObject = drawLabeledObject;
        }

        public void Draw(GeometryDrawer gd)
        {
            Draw(gd, Style);
        }

        public void Draw(GeometryDrawer gd, Style style)
        {
            var endPos = gd.DrawString(LabelText, Position, 16, style);
            Size = endPos - Position;
            endPos.X -= .1;
            gd.DrawString(Superscript, (endPos.X, Position.Y), 8, style);
            gd.DrawString(Subscript, (endPos.X, Position.Y + .3), 8, style);
        }

        public void DrawLabeledObject(GeometryDrawer gd, Style style)
        {
            drawLabeledObject(gd, style);
        }
    }
}