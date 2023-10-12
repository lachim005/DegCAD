using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    internal class PointCoordInput2D : IGeometryCommand
    {
        public Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Vynést body";
            esb.CommandHelp = "Zadejte souřadnice a názvy bodů, které chcete vynést";

            //The dialog is used to enter the point coordinates
            var dialog = new PointCoordinateInputDialog2D();
            dialog.ShowDialog();

            if (dialog.Canceled)
            {
                return Task.FromResult<TimelineItem?>(null);
            }

            List<IMongeItem> pts = new();

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            foreach (var pt in dialog.Points)
            {
                //Adds the point
                pts.Add(new Point(pt.X, -pt.Y, curStyle, vpl));
                //Adds the label
                if (!string.IsNullOrWhiteSpace(pt.Label))
                {
                    pts.Add(new Label(pt.Label, "", "", (pt.X, -pt.Y), curStyle, new Point(pt.X, -pt.Y), vpl));
                }
            }

            return Task.FromResult<TimelineItem?>(new(pts.ToArray()));
        }
    }
}
