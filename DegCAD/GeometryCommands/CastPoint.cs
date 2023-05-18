using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DegCAD.GeometryCommands
{
    public class CastPoint : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            var blueStyle = new Style() { Color = Colors.Blue, LineStyle = 1 };
            var redStyle = new Style() { Color = Colors.Red };
            var greenStyle = new Style() { Color = Colors.Green };
            esb.CommandName = "Sklopit bod";

            //Get point to cast
            esb.CommandHelp = "Vyberte bod, který chcete sklopit";
            var point = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt, Style.Default);
            });

            var circle = new Circle2(point, 0);
            var secondProjectionLine = new ParametricLine2(point, (1, 0));

            //Get the second projection of the point
            esb.CommandHelp = "Vyberte druhý průmět bodu, který chcete sklopit";
            var secondProjection = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(point, Style.Default);

                secondProjectionLine.Point = pt;
                gd.DrawLine(secondProjectionLine, double.NegativeInfinity, double.PositiveInfinity, redStyle);
                gd.DrawPointCross(pt, Style.Default);

                circle.Radius = Math.Abs(pt.Y);
                gd.DrawCircle(circle, blueStyle);
            });

            //Get the direction
            esb.CommandHelp = "Vyberte směr, kterým chcete bod sklopit, pravým tlačítkem přepnete kolmý směr";
            (var dirPoint, var perpendicular) = await inputMgr.GetPointWithPlane((pt, gd, perp) =>
            {
                gd.DrawLine(secondProjectionLine, double.NegativeInfinity, double.PositiveInfinity, redStyle);
                gd.DrawPointCross(pt, Style.Default);

                gd.DrawCircle(circle, blueStyle);

                var ptOnCircle = circle.TranslatePointToCircle(pt);
                var sth = ptOnCircle - point;

                if (perp)
                {
                    gd.DrawLine(ptOnCircle, point - sth, greenStyle);
                    //Calculates the perpendicular coordinate of the point
                    Vector2 perpVec = new(sth.Y, -sth.X);
                    var perpPtOnCircle = point - perpVec;
                    gd.DrawLine(perpPtOnCircle, point, redStyle);
                    gd.DrawPointCross(perpPtOnCircle, redStyle);
                }
                else
                {
                    gd.DrawLine(ptOnCircle, point, redStyle);
                    gd.DrawPointCross(ptOnCircle, redStyle);
                }
            }, defaultPlane: true , circles: new Circle2[1] { circle }, predicate: (pt) => pt != point);

            List<IMongeItem> mItems = new();

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            //Get the point result
            var ptOnCircle = circle.TranslatePointToCircle(dirPoint);
            if (perpendicular)
            {
                //Calculates the perpendicular coordinate of the point
                var sth = ptOnCircle - point;
                Vector2 perpVec = new(sth.Y, -sth.X);
                ptOnCircle = point - perpVec;
                //Adds a line connecting the casted point and the original point
                mItems.Add(new MongeItems.LineSegment(ptOnCircle, point, curStyle));
            }

            var mPoint = new DrawableItems.Point(ptOnCircle.X, ptOnCircle.Y, curStyle);
            mItems.Add(mPoint);

            //Label
            esb.CommandHelp = "Zadejte název sklopeného bodu";
            LabelInput lid = new();
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, mPoint.Coords, curStyle, mPoint));
            }

            return new TimelineItem(mItems.ToArray());
        }
    }
}
