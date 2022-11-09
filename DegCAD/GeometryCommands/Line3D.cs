using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class Line3D : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr)
        {
            ParametricLine2 line1 = new();
            int line1Sign = 1;
            ParametricLine2 line2 = new();
            int line2Sign = 1;

            //First projection
            (Vector2 pt1, bool plane) = await inputMgr.GetPointWithPlane((p, gd, plane) =>
            {
                gd.DrawPlane(plane);
                
                gd.DrawPointCross(p, Style.Default);
            });
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

            //Second projection
            Vector2 pt3 = await inputMgr.GetPoint((p, gd) =>
            {
                gd.DrawPlane(!plane);

                gd.DrawPointCross(pt1, Style.Default);
                gd.DrawPointCross(pt2, Style.Default);
                gd.DrawPointCross(p, Style.Default);

                gd.DrawLine(line1, double.PositiveInfinity * line1Sign, line1.GetParamFromY(0), Style.Default);
            });
            Vector2 pt4 = await inputMgr.GetPoint((p, gd) =>
            {
                gd.DrawPlane(!plane);

                gd.DrawPointCross(pt1, Style.Default);
                gd.DrawPointCross(pt2, Style.Default);
                gd.DrawPointCross(pt3, Style.Default);
                gd.DrawPointCross(p, Style.Default);

                gd.DrawLine(line1, double.PositiveInfinity * line1Sign, line1.GetParamFromY(0), Style.Default);

                line2 = ParametricLine2.From2Points(pt3, p);

                //Calculates the infinity sign
                line2Sign = !plane ? -1 : 1;
                if (line2.DirectionVector.Y * line2.DirectionVector.X < 0)
                {
                    line2.DirectionVector = -line2.DirectionVector;
                    line2Sign *= -1;
                }

                gd.DrawLine(line2, double.PositiveInfinity * line2Sign, line2.GetParamFromY(0), Style.Default);
            });

            Line line;
            if (plane)
            {
                line = new Line() { BottomProjectionLine = line2, TopProjectionLine = line1 };
            } else
            {
                line = new Line() { BottomProjectionLine = line1, TopProjectionLine = line2 };
            }

            return new TimelineItem(
                new IMongeItem[1] {line},
                new Vector2[0]
            );
        }
    }
}
