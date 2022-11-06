using System;
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

        /// <summary>
        /// Creates a new instance of the geometry drawer class
        /// </summary>
        /// <param name="vp">The viewport canvas that this GD will draw to</param>
        public GeometryDrawer(ViewPort vp)
        {
            this.vp = vp;
        }

        /// <summary>
        /// Clears the canvas
        /// </summary>
        public void Clear()
        {
            vp.WBmp.Clear(Colors.Transparent);
        }

        /// <summary>
        /// Draws a parametric line from two parameters. Parameters can be infinity
        /// </summary>
        /// <param name="line">The parametric line</param>
        /// <param name="from">The parameter of the beginning point of the line</param>
        /// <param name="to">The parameter of the ending point of the line</param>
        /// <param name="s">Style of the line</param>
        public void DrawLine(ParametricLine2 line, double from, double to, Style s, int thickness = 1)
        {
            //Calculates the canvas points the line will be drawn between
            var p1 = line.GetPoint(ClampInfinity(line, from));
            var p2 = line.GetPoint(ClampInfinity(line, to));

            DrawLine(p1, p2, s, thickness);
        }

        /// <summary>
        /// Draws a line between two points in canvas space
        /// </summary>
        public void DrawLine(Vector2 p1, Vector2 p2, Style s, int thickness = 1)
        {
            //Translates the coordinates to screen space and multiplies them by the scaling factor
            var sp1 = vp.CanvasToScreen(p1);
            var sp2 = vp.CanvasToScreen(p2);
                                                          
            switch (s.LineStyle)
            {
                case 1:
                    //Dashed line
                    vp.WBmp.DrawLineDotted((int)sp1.X, (int)sp1.Y, (int)sp2.X, (int)sp2.Y, 5, 5, s.Color);
                    break;
                default:
                    //Solid line
                    vp.WBmp.DrawLineAa((int)sp1.X, (int)sp1.Y, (int)sp2.X, (int)sp2.Y, s.Color, thickness);
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
                vp.WBmp.FillRectangle(0, (int)vp.CanvasToScreen((0, 0)).Y, vp.CWidth, vp.CHeight, Color.FromRgb(255, 255, 200));
            else
                vp.WBmp.FillRectangle(0, (int)vp.CanvasToScreen((0, 0)).Y, vp.CWidth, -1, Color.FromRgb(255, 255, 200));
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
            int w = vp.WBmp.PixelWidth;
            int h = vp.WBmp.PixelHeight;
            var bm2 = new System.Drawing.Bitmap(
                    w, h, vp.WBmp.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppArgb,
                    vp.WBmp.BackBuffer);

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
                vp.WBmp.FillRectangle((int)screenCoords.X, (int)screenCoords.Y,
                    (int)(screenCoords.X + sizeVector.X), (int)(screenCoords.Y + sizeVector.Y),
                    Colors.Transparent);

                //Draws the text
                g.DrawString(text, font, brush, (int)screenCoords.X, (int)screenCoords.Y);

                //Returns the end position of the text
                return vp.ScreenToCanvas(sizeVector + screenCoords);
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
