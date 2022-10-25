using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class Point3D : IGeometryCommand
    {
        public async void ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr)
        {
            Style previewStyle = new() { Color = Color.FromRgb(0, 0, 255), LineStyle = 1 };
            ParametricLine2 xLine = new((0, 0), (0, 1));

            var c1 = await inputMgr.GetPoint((p, gd) =>
            {
                //X line
                xLine.Point = p;
                gd.DrawLine(xLine, double.NegativeInfinity, double.PositiveInfinity, previewStyle);
                //Point 1 cross
                gd.DrawLine((p.X - .2, p.Y), (p.X + .2, p.Y), Style.Default);
                gd.DrawLine((p.X, p.Y - .2), (p.X, p.Y + .2), Style.Default);
            });

            var c2 = await inputMgr.GetPoint((p, gd) =>
            {
                //X line
                gd.DrawLine(xLine, double.NegativeInfinity, double.PositiveInfinity, previewStyle);
                //Point 1 cross
                gd.DrawLine((c1.X - .2, c1.Y), (c1.X + .2, c1.Y), Style.Default);
                gd.DrawLine((c1.X, c1.Y - .2), (c1.X, c1.Y + .2), Style.Default);
                //Point 2 cross
                gd.DrawLine((c1.X - .2, p.Y), (c1.X + .2, p.Y), Style.Default);
                gd.DrawLine((c1.X, p.Y - .2), (c1.X, p.Y + .2), Style.Default);
            });
        }
    }
}
