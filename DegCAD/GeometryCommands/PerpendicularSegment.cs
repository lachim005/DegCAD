using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = DegCAD.MongeItems.Point;

namespace DegCAD.GeometryCommands
{
    public class PerpendicularSegment : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Kolmá úsečka";

            esb.CommandHelp = "Vyberte přímku, ke které chcete sestrojit kolmou úsečku";

            Line selectedLine = new();
            selectedLine.SetStyle(Style.HighlightStyle);
            previewVpl.Canvas.Children.Add(selectedLine);

            ParametricLine2 line = await inputMgr.GetLine((p, l) =>
            {
                if (l is not null)
                {
                    selectedLine.SetParaLine(previewVpl, (ParametricLine2)l, double.NegativeInfinity, double.PositiveInfinity);
                    selectedLine.Visibility = Visibility.Visible;
                }
                else
                {
                    selectedLine.Visibility = Visibility.Hidden;
                }
            });

            esb.CommandHelp = "Vyberte bod, kterým bude kolmice procházet";

            line.DirectionVector = (line.DirectionVector.Y, -line.DirectionVector.X);
            selectedLine.SetStyle(Style.BlueDashStyle);
            selectedLine.Visibility = Visibility.Visible;
            Point mLinePoint = new(0, 0, previewVpl);

            Vector2 linePoint = await inputMgr.GetPoint((pt) =>
            {
                line.Point = pt;
                mLinePoint.Coords = pt;
                mLinePoint.Draw();
                selectedLine.SetParaLine(previewVpl, line, double.NegativeInfinity, double.PositiveInfinity);
            });

            esb.CommandHelp = "Vyberte první bod úsečky";

            Point mSegPt1 = new(0,0, previewVpl);

            var p1 = await inputMgr.GetPoint((pt) =>
            {
                pt = line.GetClosestPoint(pt);
                mLinePoint.Draw();

                mSegPt1.Coords = pt;
                mSegPt1.Draw();
                selectedLine.SetParaLine(previewVpl, line, double.NegativeInfinity, double.PositiveInfinity);
            }, lines: new ParametricLine2[1] { line });

            p1 = line.GetClosestPoint(p1);

            esb.CommandHelp = "Vyberte druhý bod úsečky";

            Point mSegPt2 = new(0, 0, previewVpl);
            MongeItems.LineSegment mLineSeg = new(p1, p1, Style.HighlightStyle, previewVpl);

            var p2 = await inputMgr.GetPoint((pt) =>
            {
                pt = line.GetClosestPoint(pt);
                mLinePoint.Draw();

                mSegPt2.Coords = pt;
                mSegPt2.Draw();
                mSegPt1.Draw();

                mLineSeg.P2 = pt;
                mLineSeg.Draw();

                selectedLine.SetParaLine(previewVpl, line, double.NegativeInfinity, double.PositiveInfinity);
            }, lines: new ParametricLine2[1] { line });

            p2 = line.GetClosestPoint(p2);

            return new([new MongeItems.LineSegment(p1, p2, inputMgr.StyleSelector.CurrentStyle, vpl)]);
        }
    }
}
