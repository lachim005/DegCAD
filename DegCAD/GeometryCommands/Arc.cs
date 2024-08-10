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

            Vector2 center = await inputMgr.GetPoint((pt) =>
            {
                mCenterPt.Coords = pt;
                mCenterPt.Draw();
            });

            esb.CommandHelp = "Vyberte průměr kružnice, na které bude oblouk ležet";

            Point mCirclePt = new(0, 0, previewVpl);
            MongeItems.Circle mCircle = new(new(), Style.BlueDashStyle, previewVpl);

            Vector2 radiusPoint = await inputMgr.GetPoint((pt) =>
            {
                mCenterPt.Draw();

                mCirclePt.Coords = pt;
                mCirclePt.Draw();

                mCircle.Circle2 = new(center, pt);
                mCircle.Draw();
            }, predicate: (pt) => pt != center);

            Circle2 circle = new(center, radiusPoint);


            esb.CommandHelp = "Vyberte počáteční bod oblouku";

            MongeItems.LineSegment mSeg1 = new(center, (0, 0), Style.HighlightStyle, previewVpl);
            MongeItems.Arc mArc = new(circle, 0, 1, Style.HighlightStyle, previewVpl);
            Point mStartPt = new(0,0, previewVpl);

            Vector2 pt1 = await inputMgr.GetPoint((pt) =>
            {
                mCenterPt.Draw();
                mCirclePt.Draw();
                mCircle.Draw();

                pt = circle.TranslatePointToCircle(pt);
                mStartPt.Coords = pt;
                mStartPt.Draw();

                mSeg1.P2 = pt;
                mSeg1.Draw();
            }, 
            circles: [circle],
            lines: [new(center, (1, 0)), new(center, (0, 1))], // Snapping vertical and horizontal lines
            predicate: (pt) => pt != center);

            Vector2 startPoint = circle.TranslatePointToCircle(pt1);
            Vector2 startVec = startPoint - center;

            mSeg1.P2 = startPoint;

            //Calculates the start angle and adjusts it by it's quadrant
            double startAngle = Math.Atan(startVec.Y / startVec.X);
            if (startVec.X < 0) startAngle += Math.PI;
            else if (startVec.Y < 0) startAngle += Math.PI * 2;

            double endAngle = 0;

            esb.CommandHelp = "Vyberte koncový bod oblouku, pravým tlačítkem myši změníte směr oblouku";

            MongeItems.LineSegment mSeg2 = new(center, (0, 0), Style.HighlightStyle, previewVpl);
            Point mEndPt = new(0, 0, previewVpl);

            (var pt2, var swap) = await inputMgr.GetPointWithPlane((pt, swap) =>
            {
                mCenterPt.Draw();
                mCirclePt.Draw();
                mCircle.Draw();
                mStartPt.Draw();

                //Calculates a point on the circle in the direction set by the current point
                var endPoint = circle.TranslatePointToCircle(pt);
                var endVec = endPoint - center;


                mEndPt.Coords = endPoint;
                mEndPt.Draw();

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

                mArc.Draw();

                mSeg1.Draw();
                mSeg2.P2 = endPoint;
                mSeg2.Draw();
            }, 
            circles: [circle],
            lines: [new(center, startVec), new(center, (startVec.Y, -startVec.X))], // Snapping arc length at 90° increments
            predicate: (pt) => pt != center);

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            //Swaps the start and end angles if necessary
            if (swap)
                return new([new MongeItems.Arc(circle, endAngle, startAngle, curStyle, vpl)]);
            return new([new MongeItems.Arc(circle, startAngle, endAngle, curStyle, vpl)]);
        }
    }
}
