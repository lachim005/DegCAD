using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DegCAD.DrawableItems;

namespace DegCAD.GeometryCommands
{
    public class Point3D : IGeometryCommand
    {
        public Vector2 p1;
        public Vector2 p2;

        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr)
        {
            Style previewStyle = new() { Color = Color.FromRgb(0, 0, 255), LineStyle = 1 };
            ParametricLine2 xLine = new((0, 0), (0, 1));

            p1 = await inputMgr.GetPoint((p, gd) =>
            {
                //X line
                xLine.Point = p;
                gd.DrawLine(xLine, double.NegativeInfinity, double.PositiveInfinity, previewStyle);
                //Point 1 cross
                gd.DrawLine((p.X - .2, p.Y), (p.X + .2, p.Y), Style.Default);
                gd.DrawLine((p.X, p.Y - .2), (p.X, p.Y + .2), Style.Default);
            });

            p2 = await inputMgr.GetPoint((p, gd) =>
            {
                //X line
                gd.DrawLine(xLine, double.NegativeInfinity, double.PositiveInfinity, previewStyle);
                //Point 1 cross
                gd.DrawLine((p1.X - .2, p1.Y), (p1.X + .2, p1.Y), Style.Default);
                gd.DrawLine((p1.X, p1.Y - .2), (p1.X, p1.Y + .2), Style.Default);
                //Point 2 cross
                gd.DrawLine((p1.X - .2, p.Y), (p1.X + .2, p.Y), Style.Default);
                gd.DrawLine((p1.X, p.Y - .2), (p1.X, p.Y + .2), Style.Default);
            });

            p2.X = p1.X;

            Point mpoint = new() { X = p1.X, Y = p1.Y, Z = -p2.Y };
            return new(
                new IMongeItem[1] { mpoint }, //Drawable items
                new Vector2[2] { p1, p2 }     //Snapable points
            );
        }
    }
}
