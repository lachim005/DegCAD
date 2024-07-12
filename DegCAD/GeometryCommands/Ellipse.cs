using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class Ellipse : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Elipsa";

            esb.CommandHelp = "Vyberte střed elipsy";

            Point mCenterPt = new(0, 0, previewVpl);

            Vector2 center = await inputMgr.GetPoint((pt) =>
            {
                mCenterPt.Coords = pt;
                mCenterPt.Draw();
            });

            esb.CommandHelp = "Vyberte hlavní vrchol elipsy";

            Point mPrimaryPt = new(0, 0, previewVpl);
            MongeItems.Ellipse mEllipse = new(center, center, center, Style.HighlightStyle, previewVpl);
            MongeItems.LineSegment mPrimaryAxis = new(center, center, Style.GreenStyle,previewVpl);

            Vector2 primaryPt = await inputMgr.GetPoint((pt) =>
            {
                mPrimaryPt.Coords = pt;
                mPrimaryPt.Draw();
                mCenterPt.Draw();

                mEllipse.P1 = pt;
                mEllipse.Draw();

                var axisVec = pt - center;
                mPrimaryAxis.P1 = center + axisVec;
                mPrimaryAxis.P2 = center - axisVec;
                mPrimaryAxis.Draw();
            },
            lines: [new(center, (1, 0)), new(center, (0, 1))]); // Snapping vertical and horizontal lines

            esb.CommandHelp = "Vyberte vedlejší vrchol elipsy";

            Point mSecondaryPt = new(0, 0, previewVpl);
            MongeItems.LineSegment mSecondaryAxis = new(center, center, Style.GreenStyle, previewVpl);
            var primaryAxisVec = primaryPt - center;
            ParametricLine2 secondaryAxis = new(center, (primaryAxisVec.Y, -primaryAxisVec.X));

            Vector2 secondaryPt = await inputMgr.GetPoint((pt) =>
            {
                mSecondaryPt.Coords = pt;
                mSecondaryPt.Draw();
                mPrimaryPt.Draw();
                mCenterPt.Draw();

                mEllipse.P2 = pt;
                mEllipse.Draw();

                var axisVec = secondaryAxis.GetClosestPoint(pt) - center;
                mSecondaryAxis.P1 = center + axisVec;
                mSecondaryAxis.P2 = center - axisVec;
                mSecondaryAxis.Draw();
                mPrimaryAxis.Draw();
            },
            lines: [new(center, (1, 0)), new(center, (0, 1))]); // Snapping vertical and horizontal lines

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            return new(new IMongeItem[1] {new MongeItems.Ellipse(center, primaryPt, secondaryPt, curStyle, vpl)});
        }
    }
}
