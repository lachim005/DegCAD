using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DegCAD.GeometryCommands
{
    public class TranslateSize : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr)
        {
            //Defines some styles
            var blueStyle = new Style() { Color = Colors.Blue, LineStyle = 1 };
            var redStyle = new Style() { Color = Colors.Red };
            var greenStyle = new Style() { Color = Colors.YellowGreen };

            Vector2 pt1 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt, Style.Default);
            });

            Vector2 pt2 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt1, Style.Default);
                gd.DrawPointCross(pt, Style.Default);

                gd.DrawLine(pt1, pt, greenStyle);

            });

            var distance = pt2 - pt1;

            Vector2 pt3 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt1, Style.Default);
                gd.DrawPointCross(pt2, Style.Default);
                gd.DrawPointCross(pt, Style.Default);

                gd.DrawLine(pt1, pt2, greenStyle);
                gd.DrawCircle(new Circle2(pt, distance.Length), blueStyle);

            });

            var circle = new Circle2(pt3, distance.Length);

            Vector2 pt4 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt1, Style.Default);
                gd.DrawPointCross(pt2, Style.Default);
                gd.DrawPointCross(pt3, Style.Default);
                gd.DrawPointCross(pt, Style.Default);

                gd.DrawLine(pt1, pt2, greenStyle);
                gd.DrawCircle(circle, blueStyle);

                var ptOnCircle = circle.TranslatePointToCircle(pt);
                gd.DrawPointCross(ptOnCircle, redStyle);
                gd.DrawLine(pt3, ptOnCircle, redStyle);
            }, predicate: (pt) => pt != pt3, circles: new Circle2[1] { circle });

            var ptOnCircle = circle.TranslatePointToCircle(pt4);

            return new(new IMongeItem[1] { new DrawableItems.Point(ptOnCircle.X, ptOnCircle.Y) });
        }
    }
}
