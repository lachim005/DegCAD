﻿using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace DegCAD.GeometryCommands
{
    public class Point2D : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Bod";

            esb.CommandHelp = "Vyberte bod";

            //Point cross
            Point mPt = new(0, 0, Style.Default, previewVpl);

            var pt = await inputMgr.GetPoint((pt) =>
            {
                mPt.Coords = pt;
                mPt.Draw();
            });

            List<IMongeItem> mongeItems = new(2)
            {
                new Point(pt.X, pt.Y, inputMgr.StyleSelector.CurrentStyle, vpl)
            };

            esb.CommandHelp = "Zadejte název bodu";
            var lid = new LabelInput();
            lid.ShowDialog();

            if (!lid.Canceled)
            {
                mongeItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, pt, inputMgr.StyleSelector.CurrentStyle, mongeItems[0].Clone(), vpl, lid.TextSize));
            }
            return new(mongeItems.ToArray());
        }
    }
}
