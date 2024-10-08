﻿using DegCAD.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using Point = DegCAD.TimelineElements.Point;
using DegCAD.TimelineElements;

namespace DegCAD.GeometryCommands
{
    public class TranslateHeight : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Nanést výšku";
            esb.CommandHelp = "Vyberte první bod výšky";

            Point mHeightPt1 = new(0, 0, previewVpl);

            var hp1 = await inputMgr.GetPoint((pt) =>
            {
                mHeightPt1.Coords = pt;
                mHeightPt1.Draw();
            });

            esb.CommandHelp = "Vyberte druhý bod výšky";

            TimelineElements.LineSegment mGivenHeightSeg = new(hp1, hp1, Style.GreenStyle, previewVpl);
            Point mHeightPt2 = new(hp1.X, hp1.Y, previewVpl);

            var hp2 = await inputMgr.GetPoint((pt) =>
            {
                mHeightPt2.Y = pt.Y;
                mGivenHeightSeg.P2 = (hp1.X, pt.Y);

                mHeightPt1.Draw();
                mHeightPt2.Draw();
                mGivenHeightSeg.Draw();
            },
            lines: [new(hp1, (0,1))]); // Vertical snapping

            esb.CommandHelp = "Vyberte bod, do kterého chcete přenést výšku, pravým tlačítkem změníte směr výšky";

            double height = hp2.Y - hp1.Y;

            TimelineElements.LineSegment mheightSeg = new(hp1, hp1, Style.BlueDashStyle, previewVpl);
            Point mResultPt = new(hp1.X, hp1.Y, Style.HighlightStyle, previewVpl);
            Point mResultStartPt = new(hp1.X, hp1.Y, previewVpl);

            (var pt, var dir) = await inputMgr.GetPointWithPlane((pt, dir) =>
            {
                mheightSeg.P1 = pt;
                mheightSeg.P2 = (pt.X, pt.Y + (dir? -height : +height));
                mResultPt.Coords = mheightSeg.P2;
                mResultStartPt.Coords = pt;

                mHeightPt1.Draw();
                mHeightPt2.Draw();
                mGivenHeightSeg.Draw();
                mResultPt.Draw();
                mResultStartPt.Draw();
                mheightSeg.Draw();
            });

            Vector2 res = (pt.X, pt.Y + (dir ? -height : +height));

            var curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<GeometryElement> mItems =
            [
                new Point(res.X, res.Y, curStyle, vpl)
            ];

            if (inputMgr.NameNewItems)
            {
                esb.CommandHelp = "Zadejte název bodu";
                LabelInput lid = new();
                lid.ShowDialog();
                if (!lid.Canceled)
                {
                    mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, res, curStyle, mItems[0].CloneElement(), vpl, lid.TextSize));
                } 
            }

            return new([.. mItems]);
        }
    }
}
