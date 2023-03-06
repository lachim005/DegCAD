﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DegCAD
{
    /// <summary>
    /// A class that encapsulates the drawing to the viewport
    /// </summary>
    public class GeometryDrawer
    {
        readonly ViewPort vp;
        private WriteableBitmap wBmp;
        private bool preview;

        /// <summary>
        /// Creates a new instance of the geometry drawer class
        /// </summary>
        /// <param name="vp">The viewport canvas that this GD will draw to</param>
        public GeometryDrawer(ViewPort vp, bool preview)
        {
            this.vp = vp;
            this.preview = preview;
            vp.SizeChanged += VpSizeChanged;
            if (preview)
                wBmp = vp.PreviewWBmp;
            else
                wBmp = vp.GeometryWBmp;
        }

        private void VpSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (preview)
                wBmp = vp.PreviewWBmp;
            else
                wBmp = vp.GeometryWBmp;
        }

        /// <summary>
        /// Clears the canvas
        /// </summary>
        public void Clear()
        {
            wBmp.Clear(Colors.Transparent);
            if (preview) vp.BackgroundWBmp.Clear();
        }

        /// <summary>
        /// Draws a parametric line from two parameters. Parameters can be infinity
        /// </summary>
        /// <param name="line">The parametric line</param>
        /// <param name="from">The parameter of the beginning point of the line</param>
        /// <param name="to">The parameter of the ending point of the line</param>
        /// <param name="s">Style of the line</param>
        public void DrawLine(ParametricLine2 line, double from, double to, Style s)
        {
            if (double.IsNaN(from))
                from = -to;
            if (double.IsNaN(to))
                to = -from;

            //Calculates the canvas points the line will be drawn between
            var p1 = line.GetPoint(ClampInfinity(line, from));
            var p2 = line.GetPoint(ClampInfinity(line, to));

            DrawLine(p2, p1, s);
        }

        /// <summary>
        /// Draws a line between two points in canvas space
        /// </summary>
        public void DrawLine(Vector2 p1, Vector2 p2, Style s)
        {
            //Translates the coordinates to screen space and multiplies them by the scaling factor
            var sp1 = vp.CanvasToScreen(p1);
            var sp2 = vp.CanvasToScreen(p2);
                                                          
            switch (s.LineStyle)
            {
                case 1:
                    //Dashed line
                    wBmp.DrawLineDashed((int)sp1.X, (int)sp1.Y, (int)sp2.X, (int)sp2.Y, s.Color);
                    break;
                case 2:
                    //Dashed line
                    wBmp.DrawLineDotDash((int)sp1.X, (int)sp1.Y, (int)sp2.X, (int)sp2.Y, s.Color);
                    break;
                default:
                    //Solid line
                    wBmp.DrawLineSolid((int)sp1.X, (int)sp1.Y, (int)sp2.X, (int)sp2.Y, s.Color);
                    break;
            }
        }
        /// <summary>
        /// Fills one plane
        /// </summary>
        /// <param name="plane">False to draw the bottom plane, true for the top plane</param>
        public void DrawPlane(bool plane)
        {
            if (!plane)
                vp.BackgroundWBmp.FillRectangle(0, (int)vp.CanvasToScreen((0, 0)).Y, vp.CWidth, vp.CHeight, Color.FromRgb(255, 255, 200));
            else
                vp.BackgroundWBmp.FillRectangle(0, (int)vp.CanvasToScreen((0, 0)).Y, vp.CWidth, -1, Color.FromRgb(255, 255, 200));
        }
        /// <summary>
        /// Draws a cross on the set point
        /// </summary>
        public void DrawPointCross(Vector2 pt, Style style, double size = 0.2)
        {
            var solidStyle = style;
            solidStyle.LineStyle = 0;
            DrawLine((pt.X - size, pt.Y), (pt.X + size, pt.Y), solidStyle);
            DrawLine((pt.X, pt.Y - size), (pt.X, pt.Y + size), solidStyle);
        }
        /// <summary>
        /// Draws a string on the canvas in canvas space
        /// </summary>
        /// <returns>The canvas space position where the text ended so you can continue drawing something like the lower index</returns>
        public Vector2 DrawString(string text, Vector2 coords, int fontSize, Style s)
        {
            
            Vector2 screenCoords = vp.CanvasToScreen(coords);

            //I have no idea how this works, but it works

            //Creates a new Bitmap that has a method for drawing text
            int w = wBmp.PixelWidth;
            int h = wBmp.PixelHeight;
            var bm2 = new System.Drawing.Bitmap(
                    w, h, wBmp.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppArgb,
                    wBmp.BackBuffer);

            using (var g = System.Drawing.Graphics.FromImage(bm2))
            {
                //Creates a brush for the text
                var col = System.Drawing.Color.FromArgb(s.Color.A, s.Color.R, s.Color.G, s.Color.B);
                var brush = new System.Drawing.SolidBrush(col);

                //Creates a font and a fontsize
                int fs = (int)(fontSize * vp.Scale);
                if (fs < 1) fs = 1;
                var font = new System.Drawing.Font("Tahoma", fs, System.Drawing.FontStyle.Regular);

                //Gets the size of the text being drawn and converts it to a vector
                var size = g.MeasureString(text, font);
                Vector2 sizeVector = (size.Width, size.Height);

                //Changes some settings to make the font look nicer
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                //Fills a transparent rectangle below the text so it doesn't get obstructed by lines
                /*wBmp.FillRectangle((int)screenCoords.X, (int)screenCoords.Y,
                    (int)(screenCoords.X + sizeVector.X), (int)(screenCoords.Y + sizeVector.Y),
                    Colors.Transparent);*/

                //Draws the text
                g.DrawString(text, font, brush, (int)screenCoords.X, (int)screenCoords.Y);

                //Returns the end position of the text
                return vp.ScreenToCanvas(sizeVector + screenCoords);
            }
        }

        /// <summary>
        /// Draws a circle defined by it's center and a point on it
        /// </summary>
        public void DrawCircle(Vector2 middle, Vector2 point, Style s)
        {
            DrawCircle(new Circle2(middle, point), s);
        }

        /// <summary>
        /// Draws a circle
        /// </summary>
        public void DrawCircle(Circle2 circle, Style s)
        {
            var sMiddle = vp.CanvasToScreen(circle.Center);
            var sRadius = vp.Scale * ViewPort.unitSize * circle.Radius;

            switch (s.LineStyle)
            {
                case 1:
                    //Dashed circle
                    wBmp.DrawCircleDashed(sMiddle, (int)sRadius, s.Color);
                    break;
                case 2:
                    //Dot-dashed circle
                    wBmp.DrawCircleDotDash(sMiddle, (int)sRadius, s.Color);
                    break;
                default:
                    //Solid circle
                    wBmp.DrawCircle(sMiddle, (int)sRadius, s.Color);
                    break;
            }

        }

        public void DrawArc(Vector2 middle, Vector2 point, double angle, double endAngle, Style s)
        {
            DrawArc(new Circle2(middle, point), angle, endAngle, s);
        }

        public void DrawArc(Circle2 circle, double angle, double endAngle, Style s)
        {
            var sMiddle = vp.CanvasToScreen(circle.Center);
            var sRadius = vp.Scale * circle.Radius * ViewPort.unitSize;

            
            switch (s.LineStyle)
            {
                case 1:
                    //Dashed circle
                    wBmp.DrawArcDashed(sMiddle, (int)sRadius, angle, endAngle, s.Color);
                    break;
                case 2:
                    //Dot-dashed circle
                    wBmp.DrawArcDotDash(sMiddle, (int)sRadius, angle, endAngle, s.Color);
                    break;
                default:
                    //Solid circle
                    wBmp.DrawArc(sMiddle, (int)sRadius, angle, endAngle, s.Color);
                    break;
            }
        }


        /// <summary>
        /// If the parameter is infinity, clamps it to the edges of the viewport
        /// </summary>
        private double ClampInfinity(ParametricLine2 l, double t)
        {
            double par = t;
            if (t == double.NegativeInfinity)
            {
                par = l.GetParamFromX(vp.ScreenToCanvas((0, 0)).X);
                if (!double.IsFinite(par))
                {
                    return l.GetParamFromY(vp.ScreenToCanvas((0, 0)).Y);
                }
            }
            else if (t == double.PositiveInfinity)
            {
                par = l.GetParamFromX(vp.ScreenToCanvas((vp.CWidth, 0)).X);
                if (!double.IsFinite(par))
                {
                    return l.GetParamFromY(vp.ScreenToCanvas((0, vp.CHeight)).Y);
                }
            }
            return par;
        }
    }
}
