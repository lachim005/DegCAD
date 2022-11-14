using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Numerics;
using DegCAD.MongeItems;

namespace DegCAD.GeometryCommands
{
    internal class ParallelLine : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr)
        {
            Style lineSelStyle = new() { Color = Colors.Red };

            ParametricLine2 line = await inputMgr.GetLine((p, l, gd) =>
            {
                if (l is not null)
                {
                    gd.DrawLine((ParametricLine2)l, double.NegativeInfinity, double.PositiveInfinity, lineSelStyle);
                }
            });

            (Vector2 point, bool plane) = await inputMgr.GetPointWithPlane((pt, gd, plane) =>
            {
                gd.DrawPlane(plane);
                gd.DrawPointCross(pt, Style.Default);

                line.Point = pt;

                //Calculates the infinity sign
                int lineSign = plane ? -1 : 1;
                if (line.DirectionVector.Y * line.DirectionVector.X < 0)
                {
                    line.DirectionVector = -line.DirectionVector;
                    lineSign *= -1;
                }

                gd.DrawLine(line, double.PositiveInfinity * lineSign, line.GetParamFromY(0), Style.Default);
            });

            return null;
        }
    }
}
