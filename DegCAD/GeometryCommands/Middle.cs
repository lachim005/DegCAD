using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DegCAD.GeometryCommands
{
    public class Middle : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr)
        {
            Style redStyle = new() { Color = Colors.Red };

            Vector2 pt1 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt, Style.Default);
            });
            Vector2 pt2 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt1, Style.Default);
                gd.DrawPointCross(pt, Style.Default);

                gd.DrawPointCross((pt1 + pt) / 2, redStyle);
            });

            var middle = (pt1 + pt2) / 2;

            return new(new IMongeItem[1] { new DrawableItems.Point(middle.X, middle.Y) });
        }
    }
}
