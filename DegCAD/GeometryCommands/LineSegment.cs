using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class LineSegment : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr)
        {
            var p1 = await inputMgr.GetPoint((p, gd) =>
            {
                gd.DrawPointCross(p, Style.Default);
            });

            var p2 = await inputMgr.GetPoint((p, gd) =>
            {
                gd.DrawPointCross(p1, Style.Default);
                gd.DrawPointCross(p, Style.Default);
                gd.DrawLine(p1, p, Style.Default);
            });

            var lseg = new MongeItems.LineSegment(p1, p2, inputMgr.StyleSelector.CurrentStyle);

            return new(
                new IMongeItem[1] { lseg }
            );
        }
    }
}
