using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class Line2D : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Průmět přímky";

            ParametricLine2 line1 = new();
            int line1Sign = 1;

            //First projection
            esb.CommandHelp = "Vyberte první bod průmětu, pravým tlačítkem změníte průmětnu";
            (Vector2 pt1, bool plane) = await inputMgr.GetPointWithPlane((p, gd, plane) =>
            {
                gd.DrawPlane(plane);

                gd.DrawPointCross(p, Style.Default);
            });
            esb.CommandHelp = "Vyberte druhý bod průmětu, pravým tlačítkem změníte průmětnu";
            (Vector2 pt2, plane) = await inputMgr.GetPointWithPlane((p, gd, plane) =>
            {
                gd.DrawPlane(plane);

                gd.DrawPointCross(pt1, Style.Default);
                gd.DrawPointCross(p, Style.Default);

                line1 = ParametricLine2.From2Points(pt1, p);

                //Calculates the infinity sign
                line1Sign = plane ? -1 : 1;
                if (line1.DirectionVector.Y * line1.DirectionVector.X < 0)
                {
                    line1.DirectionVector = -line1.DirectionVector;
                    line1Sign *= -1;
                }

                gd.DrawLine(line1, double.PositiveInfinity * line1Sign, line1.GetParamFromY(0), Style.Default);
            }, plane);

           

            List<IMongeItem> mItems = new()
            {
                new LineProjection(line1, plane, inputMgr.StyleSelector.CurrentStyle)
            };

            return new TimelineItem(mItems.ToArray());
        }
    }
}
