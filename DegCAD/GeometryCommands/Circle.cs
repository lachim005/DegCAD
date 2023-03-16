using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class Circle : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Kružnice";

            esb.CommandHelp = "Vyberte střed kružnice";
            Vector2 center = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt, Style.Default);
            });

            esb.CommandHelp = "Vyberte průměr kružnice";
            Vector2 pointOnCircle = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(center, Style.Default);
                gd.DrawPointCross(pt, Style.Default);
                gd.DrawCircle(center, pt, Style.Default);
            }, predicate: (pt) => pt != center);

            return new TimelineItem(new IMongeItem[1] { new MongeItems.Circle(new Circle2(center, pointOnCircle), inputMgr.StyleSelector.CurrentStyle) });
        }
    }
}
