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
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Sklopit bod";

            //Get point to cast
            esb.CommandHelp = "Vyberte bod, který chcete sklopit";

            Point mPt1 = new(0, 0, previewVpl);

            Vector2 point = await inputMgr.GetPoint((pt) =>
            {
                mPt1.Coords = pt;
                mPt1.Draw(previewVpl);
            });

            //Get the second projection of the point
            esb.CommandHelp = "Vyberte druhý průmět bodu, který chcete sklopit";

            var circle = new Circle2(point, 0);
            MongeItems.Circle mCircle = new(circle, Style.BlueDashStyle, previewVpl);

            System.Windows.Shapes.Line secondProjectionLine = new();
            secondProjectionLine.SetStyle(Style.HighlightStyle);
            ParametricLine2 secondProjParaLine = new(point, (1, 0));
            previewVpl.Canvas.Children.Add(secondProjectionLine);

            Point mPt2 = new(0, 0, previewVpl);

            var secondProjection = await inputMgr.GetPoint((pt) =>
            {
                mPt1.Draw(previewVpl);

                circle.Radius = Math.Abs(pt.Y);
                mCircle.Circle2 = circle;
                mCircle.Draw(previewVpl);

                secondProjParaLine.Point = pt;
                secondProjectionLine.SetParaLine(previewVpl, secondProjParaLine, double.NegativeInfinity, double.PositiveInfinity);

                mPt2.Coords = pt;
                mPt2.Draw(previewVpl);
            });

            //Get the direction
            esb.CommandHelp = "Vyberte směr, kterým chcete bod sklopit, pravým tlačítkem přepnete kolmý směr";

            MongeItems.LineSegment mPerpendicularSeg = new(point, point, Style.GreenStyle, previewVpl);
            MongeItems.LineSegment mDirectionSegment = new(point, point, Style.HighlightStyle, previewVpl);

            (var dirPoint, var perpendicular) = await inputMgr.GetPointWithPlane((pt, perp) =>
            {
                mPt1.Draw(previewVpl);
                secondProjectionLine.SetParaLine(previewVpl, secondProjParaLine, double.NegativeInfinity, double.PositiveInfinity);

                mCircle.Draw(previewVpl);

                var ptOnCircle = circle.TranslatePointToCircle(pt);
                var sth = ptOnCircle - point;

                if (perp)
                {
                    mPerpendicularSeg.SetVisibility(System.Windows.Visibility.Visible);
                    mPerpendicularSeg.P1 = ptOnCircle;
                    mPerpendicularSeg.P2 = point - sth;
                    mPerpendicularSeg.Draw(previewVpl);

                    //Calculates the perpendicular coordinate of the point
                    Vector2 perpVec = new(sth.Y, -sth.X);
                    var perpPtOnCircle = point - perpVec;

                    mDirectionSegment.P2 = perpPtOnCircle;
                    mPt2.Coords = perpPtOnCircle;
                }
                else
                {
                    mPerpendicularSeg.SetVisibility(System.Windows.Visibility.Hidden);
                    mDirectionSegment.P2 = ptOnCircle;
                    mPt2.Coords = ptOnCircle;
                }

                mPt2.Draw(previewVpl);
                mDirectionSegment.Draw(previewVpl);
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
                mItems.Add(new MongeItems.LineSegment(ptOnCircle, point, curStyle, vpl));
            }

            var mPoint = new Point(ptOnCircle.X, ptOnCircle.Y, curStyle, vpl);
            mItems.Add(mPoint);

            //Label
            esb.CommandHelp = "Zadejte název sklopeného bodu";
            LabelInput lid = new();
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, mPoint.Coords, curStyle, mPoint.Clone(), vpl));
            }

            return new TimelineItem(mItems.ToArray());
        }
    }
}
