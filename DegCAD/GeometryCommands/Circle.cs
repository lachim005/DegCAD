﻿using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class Circle : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Kružnice";

            esb.CommandHelp = "Vyberte střed kružnice";

            Point mCenterPt = new(0, 0, previewVpl);

            Vector2 center = await inputMgr.GetPoint((pt) =>
            {
                mCenterPt.Coords = pt;
                mCenterPt.Draw(previewVpl);
            });

            esb.CommandHelp = "Vyberte průměr kružnice";

            Point mCirclePt = new(0, 0, previewVpl);
            MongeItems.Circle mCircle = new(new(), Style.HighlightStyle, previewVpl);

            Vector2 pointOnCircle = await inputMgr.GetPoint((pt) =>
            {
                mCenterPt.Draw(previewVpl);

                mCirclePt.Coords = pt;
                mCirclePt.Draw(previewVpl);

                mCircle.Circle2 = new(center, pt);
                mCircle.Draw(previewVpl);
            }, predicate: (pt) => pt != center);

            return new TimelineItem(new IMongeItem[1] { new MongeItems.Circle(new Circle2(center, pointOnCircle), inputMgr.StyleSelector.CurrentStyle, vpl) });
        }
    }
}
