﻿using DegCAD.Dialogs;
using DegCAD.DrawableItems;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    internal class PointCoordInput : IGeometryCommand
    {
        public Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Vynést body";
            esb.CommandHelp = "Zadejte souřadnice a názvy bodů, které chcete vynést";

            //The dialog is used to enter the point coordinates
            var dialog = new PointCoordinateInputDialog();
            dialog.ShowDialog();

            if (dialog.Canceled)
            {
                return Task.FromResult<TimelineItem?>(null);
            }

            List<IMongeItem> pts = new();

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            foreach (var pt in dialog.Points)
            {
                //Adds the Y projection
                if (double.IsFinite(pt.Y))
                {
                    pts.Add(new Point(pt.X, pt.Y, curStyle));
                    //Adds the label
                    if (!string.IsNullOrWhiteSpace(pt.Label))
                    {
                        pts.Add(new Label(pt.Label, "1", "", (pt.X, pt.Y), curStyle, new Point(pt.X, pt.Y)));
                    }
                }
                //Adds the Z projection
                if (double.IsFinite(pt.Z))
                {
                    pts.Add(new Point(pt.X, -pt.Z, curStyle));
                    //Adds the label
                    if (!string.IsNullOrWhiteSpace(pt.Label))
                    {
                        pts.Add(new Label(pt.Label, "2", "", (pt.X, -pt.Z), curStyle, new Point(pt.X, -pt.Z)));
                    }
                }
            }

            return Task.FromResult<TimelineItem?>(new(pts.ToArray()));
        }
    }
}
