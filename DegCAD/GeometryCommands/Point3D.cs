using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DegCAD.DrawableItems;
using System.Diagnostics;

namespace DegCAD.GeometryCommands
{
    public class Point3D : IGeometryCommand
    {
        public Vector2 p1;
        public Vector2 p2;
        bool firstPlane;

        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr)
        {
            Style previewStyle = new() { Color = Color.FromRgb(0, 0, 255), LineStyle = 1 };
            ParametricLine2 xLine = new((0, 0), (0, 1));

            (p1, firstPlane) = await inputMgr.GetPointWithPlane((p, gd, pl) =>
            {
                gd.DrawPlane(pl);
                //X line
                xLine.Point = p;
                gd.DrawLine(xLine, double.NegativeInfinity, double.PositiveInfinity, previewStyle);
                //Point 1 cross
                gd.DrawPointCross(p, Style.Default);
            });

            p2 = await inputMgr.GetPoint((p, gd) =>
            {
                gd.DrawPlane(!firstPlane);
                //X line
                gd.DrawLine(xLine, double.NegativeInfinity, double.PositiveInfinity, previewStyle);
                //Point 1 cross
                gd.DrawPointCross(p1, Style.Default);
                //Point 2 cross
                gd.DrawPointCross((p1.X, p.Y), Style.Default);
            }, new(p1, (0,1)));

            p2.X = p1.X;


            Point mpoint;
            //Sets the Y and Z coordinates depending on the first plane selected
            if (firstPlane)
            {
                mpoint = new(p1.X, p2.Y, -p1.Y);
            } else
            {
                mpoint = new(p1.X, p1.Y, -p2.Y);
            }

            return new(
                new IMongeItem[1] { mpoint }
            );
        }
    }
}
