﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
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
        #region Shape
        /// <summary>
        /// Sets the style of the shape
        /// </summary>
        public static void SetStyle(this Shape s, Style style)
        {
            s.Stroke = new SolidColorBrush(style.Color);
            s.StrokeThickness = style.Thickness + 1;
            s.StrokeDashArray = new(style.StrokeDashArray);
            s.StrokeStartLineCap = PenLineCap.Round;
            s.StrokeDashCap = PenLineCap.Round;
            s.StrokeEndLineCap = PenLineCap.Round;
            s.SnapsToDevicePixels = true;
        }
        #endregion

        #region Line
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
        public static void SetEllipse(this Ellipse el, ViewportLayer vpl, Vector2 center, Vector2 p1, Vector2 p2)
        {
            center = vpl.Viewport.CanvasToScreen(center);
            p1 = vpl.Viewport.CanvasToScreen(p1);
            p2 = vpl.Viewport.CanvasToScreen(p2);



            var l1vec = p1 - center;
            ParametricLine2 axis = new(center, (l1vec.Y, -l1vec.X));
            var l2vec = axis.GetClosestPoint(p2) - center;

            el.Width = l1vec.Length * 2;
            el.Height = l2vec.Length * 2;


            Canvas.SetLeft(el, center.X - l1vec.Length);
            Canvas.SetTop(el, center.Y - l2vec.Length);


            //Calculates the end angle and adjusts it by it's quadrant
            var endAngle = Math.Atan(l1vec.Y / l1vec.X) / Math.PI * 180;
            if (l1vec.X < 0) endAngle += 180;
            else if (l1vec.Y < 0) endAngle += 360;

            if (el.RenderTransform is not RotateTransform rotTr)
                return;
            
            rotTr.CenterX = el.Width/2;
            rotTr.CenterY = el.Height/2;
            double angle = DateTime.Now.Second;
            rotTr.Angle = endAngle;
        }
        #endregion

        #region Path
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
        /// <summary>
        /// Sets the data of the path to fit a parabola
        /// </summary>
        public static void SetParabola(this Path pth, ViewportLayer vpl, TimelineElements.Parabola pb)
        {
            var ep1 = vpl.Viewport.CanvasToScreen(pb.EndPoint1);
            var ep2 = vpl.Viewport.CanvasToScreen(pb.EndPoint2);
            var cp = vpl.Viewport.CanvasToScreen(pb.ControlPoint);

            string para = $"M {ep1.X} {ep1.Y} Q {cp.X} {cp.Y} {ep2.X} {ep2.Y}".Replace(',', '.');

            pth.Data = Geometry.Parse(para);
        }
        /// <summary>
        /// Sets the data of the path to fit an infinite parabola
        /// </summary>
        public static void SetInfiniteParabola(this Path pth, ViewportLayer vpl, TimelineElements.Parabola pb)
        {
            var vertex = vpl.Viewport.CanvasToScreen(pb.Vertex);
            var focus = vpl.Viewport.CanvasToScreen(pb.Focus);

            Vector2 v1 = vertex - focus;
            ParametricLine2 axis = new(vertex, v1);

            //Selects a corner of the screen depending on the slope of the axis
            Vector2 end;
            if (Math.Sign(v1.X) == 1 && Math.Sign(v1.Y) == 1)
                end = (0, 0);
            else if (Math.Sign(v1.X) == -1 && Math.Sign(v1.Y) == 1)
                end = (vpl.Viewport.ActualWidth, 0);
            else if (Math.Sign(v1.X) == -1 && Math.Sign(v1.Y) == -1)
                end = (vpl.Viewport.ActualWidth, vpl.Viewport.ActualHeight);
            else if (Math.Sign(v1.X) == 1 && Math.Sign(v1.Y) == -1)
                end = (0, vpl.Viewport.ActualHeight);
            else if (Math.Sign(v1.X) == -1 || Math.Sign(v1.Y) == -1)
                end = (vpl.Viewport.ActualWidth, vpl.Viewport.ActualHeight);
            else end = (0, 0);



            ParametricLine2 controlLine = new(vertex + v1, (v1.Y, -v1.X));
            double rad = (end - controlLine.GetClosestPoint(end)).Length;
            Circle2 c1 = new(focus, rad);
            ParametricLine2 endLine = new(end, controlLine.DirectionVector);
            var iPts = c1.FindIntersections(endLine);

            Vector2 ep1;
            Vector2 ep2;
            Vector2 cp;

            if (iPts is null)
            {
                ep1 = vertex;
                ep2 = vertex;
                cp = vertex;
            } else
            {
                ep1 = iPts.Value.Item1;
                ep2 = iPts.Value.Item2;

                var q = controlLine.GetClosestPoint(ep1);
                ParametricLine2 p = new(q, focus - q);
                ParametricLine2 vertexLine = new(vertex, controlLine.DirectionVector);
                var P = p.FindIntersection(vertexLine);
                ParametricLine2 t = new(P, ep1 - P);
                cp = axis.FindIntersection(t);
            }


            string para = $"M {ep1.X} {ep1.Y} Q {cp.X} {cp.Y} {ep2.X} {ep2.Y}".Replace(',', '.');

            pth.Data = Geometry.Parse(para);
        }
        public static void SetCatmullRomSpline(this Path pth, ViewportLayer vpl, List<Vector2> points, List<Vector2> controlPoints)
        {
            var vp = vpl.Viewport;

            StringBuilder sb = new();
            var startingPoint = vp.CanvasToScreen(points[1]);
            sb.Append("M " + startingPoint.X.ToString(CultureInfo.InvariantCulture) + " " + startingPoint.Y.ToString(CultureInfo.InvariantCulture));

            int cpc = 1;
            for (int i = 1; i < points.Count - 2; i++)
            {
                sb.Append(" C ");
                var cp1 = vp.CanvasToScreen(controlPoints[cpc++]);
                sb.Append(cp1.X.ToString(CultureInfo.InvariantCulture) + " " + cp1.Y.ToString(CultureInfo.InvariantCulture));
                sb.Append(", ");
                var cp2 = vp.CanvasToScreen(controlPoints[cpc++]);
                sb.Append(cp2.X.ToString(CultureInfo.InvariantCulture) + " " + cp2.Y.ToString(CultureInfo.InvariantCulture));
                sb.Append(", ");
                var pt = vp.CanvasToScreen(points[i + 1]);
                sb.Append(pt.X.ToString(CultureInfo.InvariantCulture) + " " + pt.Y.ToString(CultureInfo.InvariantCulture));
            }

            pth.Data = Geometry.Parse(sb.ToString());
        }
        #endregion



        private static double ClampInfinity(ParametricLine2 l, double t, ViewportLayer vpl)
        {
            if (double.IsFinite(t)) return t;

            double slope = l.DirectionVector.Y / l.DirectionVector.X;

            //Calculates which corner should be used to make the infinity t finite
            int right = Math.Sign(l.DirectionVector.X);

            if (t == double.PositiveInfinity) right = -right;

            if (slope >= -1 && slope <= 1)
            {
                if (right == 1)
                {
                    return l.GetParamFromX(vpl.Viewport.ScreenToCanvas((vpl.Viewport.CWidth, 0)).X);
                } else if (right == -1) 
                {
                    return l.GetParamFromX(vpl.Viewport.ScreenToCanvas((0, 0)).X);      
                }
            }

            int bottom = Math.Sign(l.DirectionVector.Y);
            if (t == double.PositiveInfinity) bottom *= -1;
            if (bottom == 1)
            {
                return l.GetParamFromY(vpl.Viewport.ScreenToCanvas((0, vpl.Viewport.CHeight)).Y);
            }
            return l.GetParamFromY(vpl.Viewport.ScreenToCanvas((0, 0)).Y);

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
