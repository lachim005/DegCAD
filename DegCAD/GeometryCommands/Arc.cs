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
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Oblouk";

            esb.CommandHelp = "Vyberte střed kružnice, na které bude oblouk ležet";

            Point mCenterPt = new(0, 0, previewVpl);

            Vector2 center = await inputMgr.GetPoint((pt, gd) =>
            {
                mCenterPt.Coords = pt;
                mCenterPt.Draw(previewVpl);
            });

            esb.CommandHelp = "Vyberte průměr kružnice, na které bude oblouk ležet";

            Point mCirclePt = new(0, 0, previewVpl);
            MongeItems.Circle mCircle = new(new(), Style.BlueDashStyle, previewVpl);

            Vector2 radiusPoint = await inputMgr.GetPoint((pt, gd) =>
            {
                mCenterPt.Draw(previewVpl);

                mCirclePt.Coords = pt;
                mCirclePt.Draw(previewVpl);

                mCircle.Circle2 = new(center, pt);
                mCircle.Draw(previewVpl);
            }, predicate: (pt) => pt != center);

            Circle2 circle = new(center, radiusPoint);


            esb.CommandHelp = "Vyberte počáteční bod oblouku";

            MongeItems.Arc mArc = new(circle, 0, 1, Style.HighlightStyle, previewVpl);
            Point mStartPt = new(0,0, previewVpl);

            Vector2 pt1 = await inputMgr.GetPoint((pt, gd) =>
            {
                mCenterPt.Draw(previewVpl);
                mCirclePt.Draw(previewVpl);
                mCircle.Draw(previewVpl);

                pt = circle.TranslatePointToCircle(pt);
                mStartPt.Coords = pt;
                mStartPt.Draw(previewVpl);
            }, circles: new Circle2[1] { circle }, predicate: (pt) => pt != center);

            Vector2 startPoint = circle.TranslatePointToCircle(pt1);
            Vector2 startVec = startPoint - center;

            //Calculates the start angle and adjusts it by it's quadrant
            double startAngle = Math.Atan(startVec.Y / startVec.X);
            if (startVec.X < 0) startAngle += Math.PI;
            else if (startVec.Y < 0) startAngle += Math.PI * 2;

            double endAngle = 0;

            esb.CommandHelp = "Vyberte koncový bod oblouku, pravým tlačítkem myši změníte směr oblouku";

            Point mEndPt = new(0, 0, previewVpl);

            (var pt2, var swap) = await inputMgr.GetPointWithPlane((pt, gd, swap) =>
            {
                mCenterPt.Draw(previewVpl);
                mCirclePt.Draw(previewVpl);
                mCircle.Draw(previewVpl);
                mStartPt.Draw(previewVpl);

                //Calculates a point on the circle in the direction set by the current point
                var endPoint = circle.TranslatePointToCircle(pt);
                var endVec = endPoint - center;


                mEndPt.Coords = endPoint;
                mEndPt.Draw(previewVpl);

                //Calculates the end angle and adjusts it by it's quadrant
                endAngle = Math.Atan(endVec.Y / endVec.X);
                if (endVec.X < 0) endAngle += Math.PI;
                else if (endVec.Y < 0) endAngle += Math.PI * 2;

                //Draws the arc and swaps the start and end angles if necessary
                if (swap)
                {
                    mArc.StartAngle = endAngle;
                    mArc.EndAngle = startAngle;
                }
                else
                {
                    mArc.StartAngle = startAngle;
                    mArc.EndAngle = endAngle;
                }

                mArc.Draw(previewVpl);
            }, circles: new Circle2[1] { circle }, predicate: (pt) => pt != center);

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            //Swaps the start and end angles if necessary
            if (swap)
                return new(new IMongeItem[1] { new MongeItems.Arc(circle, endAngle, startAngle, curStyle, vpl) });
            return new(new IMongeItem[1] { new MongeItems.Arc(circle, startAngle, endAngle, curStyle, vpl) });
        }
    }
}
