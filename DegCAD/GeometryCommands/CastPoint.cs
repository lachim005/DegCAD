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
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr)
        {
            var blueStyle = new Style() { Color = Colors.Blue, LineStyle = 1 };
            var redStyle = new Style() { Color = Colors.Red };
            var greenStyle = new Style() { Color = Colors.Green };

            //Get point to cast
            var point = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt, Style.Default);
            });

            var circle = new Circle2(point, 0);
            var secondProjectionLine = new ParametricLine2(point, (1, 0));

            //Get the second projection of the point
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

            //Get the point result
            var ptOnCircle = circle.TranslatePointToCircle(dirPoint);
            if (perpendicular)
            {
                //Calculates the perpendicular coordinate of the point
                var sth = ptOnCircle - point;
                Vector2 perpVec = new(sth.Y, -sth.X);
                ptOnCircle = point - perpVec;
                //Adds a line connecting the casted point and the original point
                mItems.Add(new MongeItems.LineSegment(ptOnCircle, point) { Style = new() { LineStyle = 0, Color = Colors.Black } });
            }

            var mPoint = new DrawableItems.Point(ptOnCircle.X, ptOnCircle.Y);
            mItems.Add(mPoint);

            //Label
            LabelInput lid = new();
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, mPoint.Coords, Style.Default, (gd, s) =>
                {
                    gd.DrawPointCross(mPoint.Coords, s);
                }));
            }

            return new TimelineItem(mItems.ToArray());
        }
    }
}
