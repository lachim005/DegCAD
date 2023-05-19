using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DegCAD
{
    public static class ShapesExtentionMethods
    {
        #region Line
        /// <summary>
        /// Sets the style of the line
        /// </summary>
        public static void SetStyle(this Line ln, Style style)
        {
            ln.Stroke = new SolidColorBrush(style.Color);
            ln.StrokeThickness = style.Thickness + 1;
            ln.StrokeDashArray = new(Style.StrokeDashArrays[style.LineStyle]);
        }
        /// <summary>
        /// Sets the screen coordinates of the line to fit a line segment
        /// </summary>
        public static void SetLineSegment(this Line ln, ViewportLayer vpl, Vector2 pt1, Vector2 pt2)
        {
            Vector2 screenP1 = vpl.Viewport.CanvasToScreen(pt1);
            Vector2 screenP2 = vpl.Viewport.CanvasToScreen(pt2);
            ln.X1 = screenP1.X;
            ln.Y1 = screenP1.Y;
            ln.X2 = screenP2.X;
            ln.Y2 = screenP2.Y;
        }

        /// <summary>
        /// Sets the screen coordinates of the line to fit a line projection
        /// </summary>
        public static void SetLineProjection(this Line ln, ViewportLayer vpl, ParametricLine2 pl, bool plane, int infinitySign)
        {
            Vector2 screenP1 = vpl.Viewport.CanvasToScreen(pl.GetPoint(pl.GetParamFromY(0)));
            Vector2 screenP2 = vpl.Viewport.CanvasToScreen(pl.GetPoint(ClampInfinity(pl, double.PositiveInfinity * infinitySign, vpl)));
            ln.X1 = screenP1.X;
            ln.Y1 = screenP1.Y;
            ln.X2 = screenP2.X;
            ln.Y2 = screenP2.Y;
        }
        #endregion








        private static double ClampInfinity(ParametricLine2 l, double t, ViewportLayer vpl)
        {
            double par = t;
            if (t == double.NegativeInfinity)
            {
                par = l.GetParamFromX(vpl.Viewport.ScreenToCanvas((0, 0)).X);
                if (!double.IsFinite(par))
                {
                    return l.GetParamFromY(vpl.Viewport.ScreenToCanvas((0, 0)).Y);
                }
            }
            else if (t == double.PositiveInfinity)
            {
                par = l.GetParamFromX(vpl.Viewport.ScreenToCanvas((vpl.Viewport.CWidth, 0)).X);
                if (!double.IsFinite(par))
                {
                    return l.GetParamFromY(vpl.Viewport.ScreenToCanvas((0, vpl.Viewport.CHeight)).Y);
                }
            }
            return par;
        }
    }
}
