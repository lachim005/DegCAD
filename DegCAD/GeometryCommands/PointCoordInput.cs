using DegCAD.Dialogs;
using DegCAD.DrawableItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    internal class PointCoordInput : IGeometryCommand
    {
        public Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr)
        {
            //The dialog is used to enter the point coordinates
            var dialog = new PointCoordinateInputDialog();
            dialog.ShowDialog();

            if (dialog.Canceled)
            {
                return Task.FromResult<TimelineItem?>(null);
            }

            List<IMongeItem> pts = new();

            foreach (var pt in dialog.Points)
            {
                //Adds the monge point
                pts.Add(new Point(pt.X, pt.Y, pt.Z));
            }

            return Task.FromResult<TimelineItem?>(new(pts.ToArray()));
        }
    }
}
