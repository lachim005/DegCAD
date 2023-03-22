using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DegCAD.GeometryCommands
{
    public class SegmentOnLine : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            Style blueStyle = new() { Color = Colors.Blue };
            Style redStyle = new() { Color = Colors.Red };

            esb.CommandName = "Úsečka na přímce";
            Style lineSelStyle = new() { Color = Colors.Red };

            esb.CommandHelp = "Vyberte první bod přímky, na které chcete sestrojit úsečku";
            var lp1 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt, Style.Default);
            });

            ParametricLine2 line = new()
            {
                Point = lp1
            };

            esb.CommandHelp = "Vyberte druhý bod přímky, na které chcete sestrojit úsečku";
            var lp2 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(lp1, Style.Default);
                gd.DrawPointCross(pt, Style.Default);

                line.DirectionVector = lp1 - pt;

                gd.DrawLine(line, double.PositiveInfinity, double.NegativeInfinity, blueStyle);
            }, predicate: (pt) => pt != lp1);

            blueStyle.LineStyle = 1;
            line.DirectionVector = lp1 - lp2;

            esb.CommandHelp = "Vyberte první bod úsečky";
            var p1 = await inputMgr.GetPoint((pt, gd) =>
            {
                pt = line.GetClosestPoint(pt);
                gd.DrawPointCross(pt, Style.Default);


                gd.DrawLine(line, double.PositiveInfinity, double.NegativeInfinity, blueStyle);
            }, lines: new ParametricLine2[1] { line });

            p1 = line.GetClosestPoint(p1);

            esb.CommandHelp = "Vyberte druhý bod úsečky";
            var p2 = await inputMgr.GetPoint((pt, gd) =>
            {
                pt = line.GetClosestPoint(pt);
                gd.DrawPointCross(pt, Style.Default);

                gd.DrawLine(line, double.PositiveInfinity, double.NegativeInfinity, blueStyle);
                gd.DrawLine(p1, pt, redStyle);
            }, lines: new ParametricLine2[1] { line });

            p2 = line.GetClosestPoint(p2);

            return new(new IMongeItem[1] { new MongeItems.LineSegment(p1, p2, inputMgr.StyleSelector.CurrentStyle) });
        }
    }
}
