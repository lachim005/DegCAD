using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            ln.StrokeStartLineCap = PenLineCap.Round;
            ln.StrokeDashCap = PenLineCap.Round;
            ln.StrokeEndLineCap = PenLineCap.Round;
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
        public static void SetParaLine(this Line ln, ViewportLayer vpl, ParametricLine2 pl, double from, double to)
        {
            Vector2 screenP1 = vpl.Viewport.CanvasToScreen(pl.GetPoint(ClampInfinity(pl, from, vpl)));
            Vector2 screenP2 = vpl.Viewport.CanvasToScreen(pl.GetPoint(ClampInfinity(pl, to, vpl)));
            ln.X1 = screenP1.X;
            ln.Y1 = screenP1.Y;
            ln.X2 = screenP2.X;
            ln.Y2 = screenP2.Y;
        }
        #endregion

        #region Circle
        /// <summary>
        /// Sets the style of the ellipse
        /// </summary>
        public static void SetStyle(this Ellipse el, Style style)
        {
            el.Stroke = new SolidColorBrush(style.Color);
            el.StrokeThickness = style.Thickness + 1;
            el.StrokeDashArray = new(Style.StrokeDashArrays[style.LineStyle]);
            el.StrokeStartLineCap = PenLineCap.Round;
            el.StrokeDashCap = PenLineCap.Round;
            el.StrokeEndLineCap = PenLineCap.Round;
        }
        /// <summary>
        /// Sets the screen coordinates of the ellipse to fit a circle
        /// </summary>
        public static void SetCircle(this Ellipse el, ViewportLayer vpl, Circle2 c)
        {
            Vector2 sCenter = vpl.Viewport.CanvasToScreen(c.Center);
            double radius = vpl.Viewport.Scale * ViewPort.unitSize * c.Radius;

            el.Width = 2 * radius;
            el.Height = 2 * radius;
            Canvas.SetLeft(el, sCenter.X - radius);
            Canvas.SetTop(el, sCenter.Y - radius);
        }
        #endregion

        #region Path
        /// <summary>
        /// Sets the style of the path
        /// </summary>
        public static void SetStyle(this Path pth, Style style)
        {
            pth.Stroke = new SolidColorBrush(style.Color);
            pth.StrokeThickness = style.Thickness + 1;
            pth.StrokeDashArray = new(Style.StrokeDashArrays[style.LineStyle]);
            pth.StrokeStartLineCap = PenLineCap.Round;
            pth.StrokeDashCap = PenLineCap.Round;
            pth.StrokeEndLineCap = PenLineCap.Round;
        }
        /// <summary>
        /// Sets the data of the path to fit an arc
        /// </summary>
        public static void SetArc(this Path pth, ViewportLayer vpl, Circle2 c, double startAngle, double endAngle)
        {
            double radius = vpl.Viewport.Scale * ViewPort.unitSize * c.Radius;

            var center = vpl.Viewport.CanvasToScreen(c.Center);

            var start = PolarToCartesian(center, radius, endAngle);
            var end = PolarToCartesian(center, radius, startAngle);

            startAngle *= 180/Math.PI;
            endAngle *= 180/Math.PI;

            if (endAngle < startAngle) endAngle += 360;
            var largeArcFlag = (endAngle - startAngle) <= 180 ? "0" : "1";

            string arc = $"M {start.X}#{start.Y} A {radius}#{radius} 0 {largeArcFlag} 0 {end.X}#{end.Y}".Replace(',', '.').Replace('#', ',');

            pth.Data = Geometry.Parse(arc);
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
        private static Vector2 PolarToCartesian(Vector2 center, double radius, double angleInRadians)
        {
            return (
            center.X + (radius * Math.Cos(angleInRadians)),
            center.Y + (radius * Math.Sin(angleInRadians))
            );
        }
    }
}
