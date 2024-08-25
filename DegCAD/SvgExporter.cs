using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DegCAD
{
    public static class SvgExporter
    {
        public static string ExportSvg(ViewPort vp, double w, double h, double scale, bool transparentBg, bool dark)
        {
            //Changes the culture so doubles will be written with dots
            var prevCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            StringBuilderWithIndent res = new();

            res.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" width=\"{w}\" height=\"{h}\" fill=\"none\">");
            res.Indent();
            res.AppendLine("<!-- Vygenerováno v DegCADu -->");

            var clonedVp = vp.Clone();
            clonedVp.Scale = scale / ViewPort.unitSize;
            clonedVp.Width = w;
            clonedVp.Height = h;

            Viewbox vb = new();
            vb.Child = clonedVp;
            vb.Measure(new(w, h));
            vb.Arrange(new(0, 0, w, h));
            vb.UpdateLayout();

            List<Color> colors = [];
            List<(int type, int thickness)> lineStyles = [];
            List<string> elements = [];

            foreach (var it in clonedVp.Timeline.CommandHistory)
            {
                foreach (var el in it.Items)
                {
                    if (el is not GeometryElement ge) continue;
                    if (!ge.IsVisible) continue;

                    if (ge is TimelineElements.Label lbl)
                    {
                        elements.AddRange(lbl.ToSvg());
                        continue;
                    }

                    // Gets color style
                    var col = ge.Style.Color;
                    int colIndex = colors.IndexOf(col);
                    if (colIndex == -1)
                    {
                        colIndex = colors.Count;
                        colors.Add(col);
                    }

                    // Gets line style
                    var line = (ge.Style.LineStyle, ge.Style.Thickness);
                    int lineIndex = lineStyles.IndexOf(line);
                    if (lineIndex == -1)
                    {
                        lineIndex = lineStyles.Count;
                        lineStyles.Add(line);
                    }

                    foreach (var shape in ge.Shapes)
                    {
                        elements.Add(ShapeToSvg(shape) + $"class=\"d c{colIndex} l{lineIndex}\" />");
                    }
                }
            }

            // Writing styles
            res.AppendLine("");
            res.AppendLine("<style type=\"text/css\">");
            res.Indent();
            
            res.AppendLine(".d { stroke-linejoin: round; stroke-linecap: round;}");
            res.AppendLine("");
            res.AppendLine(".text { dominant-baseline: hanging; font-family: Tahoma,Arial,sans-serif; }");
            res.AppendLine("");
            for (int i = 0; i < colors.Count; i++)
            {
                res.AppendLine($".c{i} {{ stroke: {Style.ToHex(colors[i])}; }}");
            }
            res.AppendLine("");
            for (int i = 0; i < lineStyles.Count; i++)
            {
                Style s = new() { LineStyle = lineStyles[i].type, Thickness = lineStyles[i].thickness };
                var dashArray =  s.StrokeDashArray;

                string style = $"stroke-width: {s.Thickness + 1};";

                if (s.LineStyle > 0)
                {

                    style += $" stroke-dasharray:";
                    foreach (var dash in dashArray)
                    {
                        style += ' ';
                        style += (dash * (s.Thickness + 1)).ToString();
                    }
                    style += ';';
                }

                res.AppendLine($".l{i} {{ {style} }}");
            }
            
            res.Unindent();
            res.AppendLine("</style>");

            if (!transparentBg)
            {
                res.AppendLine("");
                res.AppendLine($"<rect width=\"{w}\" height=\"{h}\" fill=\"{(dark ? "#262626" : "white")}\"/>");
            }

            res.AppendLine("");

            foreach (var el in elements)
            {
                res.AppendLine(el);
            }

            res.Unindent();
            res.AppendLine("</svg>");

            CultureInfo.CurrentCulture = prevCulture;

            return res.ToString();
        }

        /// <summary>
        /// Returns the shape represented as svg without any styling and without a closing "/>" or null
        /// </summary>
        public static string? ShapeToSvg(Shape shape)
        {
            if (shape is Line ln)
            {
                return $"<path d=\"M {ln.X1} {ln.Y1} L {ln.X2} {ln.Y2}\" ";
            }
            if (shape is Path pt)
            {
                return $"<path d=\"{pt.Data}\" ";
            }
            if (shape is Ellipse el)
            {
                double x = Canvas.GetLeft(el) + el.Width / 2;
                double y = Canvas.GetTop(el) + el.Height / 2;
                double rx = el.Width / 2;
                double ry = el.Height / 2;

                string start = $"<ellipse cx=\"{x}\" cy=\"{y}\" rx=\"{rx}\" ry=\"{ry}\" ";

                if (el.RenderTransform is not RotateTransform rt) return start;

                return start + $"transform=\"rotate({rt.Angle} {x} {y})\" ";
            }

            return null;
        }
    }
}
