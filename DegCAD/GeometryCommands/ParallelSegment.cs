using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DegCAD.GeometryCommands
{
    public class ParallelSegment : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            Style blueStyle = new Style() { Color = Colors.Blue, LineStyle = 1 };
            Style redStyle = new Style() { Color = Colors.Red };

            esb.CommandName = "Rovnoběžná úsečka";
            Style lineSelStyle = new() { Color = Colors.Red };

            esb.CommandHelp = "Vyberte přímku, ke které chcete sestrojit rovnoběžnou úsečku";
            ParametricLine2 line = await inputMgr.GetLine((p, l, gd) =>
            {
                if (l is not null)
                {
                    gd.DrawLine((ParametricLine2)l, double.NegativeInfinity, double.PositiveInfinity, lineSelStyle);
                }
            });

            esb.CommandHelp = "Vyberte bod, kterým bude rovnoběžka procházet";
            Vector2 linePoint = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt, Style.Default);

                line.Point = pt;

                gd.DrawLine(line, double.PositiveInfinity, double.NegativeInfinity, blueStyle);
            });

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
