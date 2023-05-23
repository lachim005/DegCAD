using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DegCAD.GeometryCommands
{
    public class SegmentOnLine : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Úsečka na přímce";

            esb.CommandHelp = "Vyberte první bod přímky, na které chcete sestrojit úsečku";

            Point mLinePt1 = new(0, 0, previewVpl);

            var lp1 = await inputMgr.GetPoint((pt, gd) =>
            {
                mLinePt1.Coords = pt;
                mLinePt1.Draw(previewVpl);
            });


            esb.CommandHelp = "Vyberte druhý bod přímky, na které chcete sestrojit úsečku";

            Point mLinePt2 = new(0, 0, previewVpl);
            ParametricLine2 line = new() { Point = lp1 };
            Line selectedLine = new();
            selectedLine.SetStyle(Style.BlueDashStyle);
            previewVpl.Canvas.Children.Add(selectedLine);

            var lp2 = await inputMgr.GetPoint((pt, gd) =>
            {
                mLinePt2.Coords = pt;
                mLinePt2.Draw(previewVpl);
                mLinePt1.Draw(previewVpl);

                line.DirectionVector = lp1 - pt;
                selectedLine.SetParaLine(previewVpl, line, double.NegativeInfinity, double.PositiveInfinity);
            }, predicate: (pt) => pt != lp1);

            line.DirectionVector = lp1 - lp2;

            esb.CommandHelp = "Vyberte první bod úsečky";

            Point mSegPt1 = new(0, 0, previewVpl);

            var p1 = await inputMgr.GetPoint((pt, gd) =>
            {
                pt = line.GetClosestPoint(pt);

                mSegPt1.Coords = pt;
                mSegPt1.Draw(previewVpl);

                mLinePt2.Draw(previewVpl);
                mLinePt1.Draw(previewVpl);
                selectedLine.SetParaLine(previewVpl, line, double.NegativeInfinity, double.PositiveInfinity);
            }, lines: new ParametricLine2[1] { line });

            p1 = line.GetClosestPoint(p1);

            esb.CommandHelp = "Vyberte druhý bod úsečky";

            Point mSegPt2 = new(0, 0, previewVpl);
            MongeItems.LineSegment mSeg = new(p1, p1, Style.HighlightStyle, previewVpl);

            var p2 = await inputMgr.GetPoint((pt, gd) =>
            {
                pt = line.GetClosestPoint(pt);

                mSegPt2.Coords = pt;
                mSegPt2.Draw(previewVpl);
                mSegPt1.Draw(previewVpl);

                mSeg.P2 = pt;
                mSeg.Draw(previewVpl);

                mLinePt2.Draw(previewVpl);
                mLinePt1.Draw(previewVpl);
                selectedLine.SetParaLine(previewVpl, line, double.NegativeInfinity, double.PositiveInfinity);
            }, lines: new ParametricLine2[1] { line });

            p2 = line.GetClosestPoint(p2);

            return new(new IMongeItem[1] { new MongeItems.LineSegment(p1, p2, inputMgr.StyleSelector.CurrentStyle, vpl) });
        }
    }
}
