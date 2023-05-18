using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DegCAD.GeometryCommands
{
    public class Arc : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            //Defines some styles
            var blueStyle = new Style() { Color = Colors.Blue, LineStyle = 1 };
            var redStyle = new Style() { Color = Colors.Red };
            esb.CommandName = "Oblouk";

            esb.CommandHelp = "Vyberte střed kružnice, na které bude oblouk ležet";
            Vector2 center = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt, Style.Default);
            });

            esb.CommandHelp = "Vyberte průměr kružnice, na které bude oblouk ležet";
            Vector2 radiusPoint = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(center, Style.Default);
                gd.DrawPointCross(pt, Style.Default);
                gd.DrawCircle(center, pt, blueStyle);
            }, predicate: (pt) => pt != center);

            Circle2 circle = new(center, radiusPoint);

            Vector2 startPoint = (0,0);

            esb.CommandHelp = "Vyberte počáteční bod oblouku";
            Vector2 pt1 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(center, Style.Default);
                gd.DrawPointCross(pt, Style.Default);
                gd.DrawCircle(circle, blueStyle);

                //Calculates a point on the circle in the direction set by the current point
                startPoint = circle.TranslatePointToCircle(pt);

                gd.DrawLine(center, startPoint, redStyle);

            }, circles: new Circle2[1] { circle }, predicate: (pt) => pt != center);

            Vector2 startVec = startPoint - center;

            //Calculates the start angle and adjusts it by it's quadrant
            double startAngle = Math.Atan(startVec.Y / startVec.X);
            if (startVec.X < 0) startAngle += Math.PI;
            else if (startVec.Y < 0) startAngle += Math.PI * 2;

            double endAngle = 0;

            esb.CommandHelp = "Vyberte koncový bod oblouku, pravým tlačítkem myši změníte směr oblouku";
            (var pt2, var swap) = await inputMgr.GetPointWithPlane((pt, gd, swap) =>
            {
                gd.DrawPointCross(center, Style.Default);
                gd.DrawPointCross(pt, Style.Default);
                gd.DrawCircle(circle, blueStyle);
                gd.DrawLine(center, startPoint, redStyle);

                //Calculates a point on the circle in the direction set by the current point
                var endPoint = circle.TranslatePointToCircle(pt);
                var endVec = endPoint - center;

                gd.DrawLine(center, endPoint, redStyle);

                //Calculates the end angle and adjusts it by it's quadrant
                endAngle = Math.Atan(endVec.Y / endVec.X);
                if (endVec.X < 0) endAngle += Math.PI;
                else if (endVec.Y < 0) endAngle += Math.PI * 2;

                //Draws the arc and swaps the start and end angles if necessary
                if (swap) gd.DrawArc(circle, endAngle, startAngle, redStyle);
                else gd.DrawArc(circle, startAngle, endAngle, redStyle);
            }, circles: new Circle2[1] { circle }, predicate: (pt) => pt != center);

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            //Swaps the start and end angles if necessary
            if (swap)
                return new(new IMongeItem[1] { new MongeItems.Arc(circle, endAngle, startAngle, curStyle) });
            return new(new IMongeItem[1] { new MongeItems.Arc(circle, startAngle, endAngle, curStyle) });
        }
    }
}
