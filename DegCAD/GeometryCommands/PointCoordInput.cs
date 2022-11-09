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
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr)
        {
            //The dialog is used to enter the point coordinates
            var dialog = new PointCoordinateInputDialog();
            dialog.ShowDialog();

            if (dialog.Canceled)
            {
                return null;
            }

            List<IMongeItem> pts = new();
            List<Vector2> snap = new();

            foreach (var pt in dialog.Points)
            {
                //Adds the monge point
                pts.Add(new Point(pt.X, pt.Y, pt.Z));

                //Adds snapable points
                if (!double.IsNaN(pt.Y))
                    snap.Add((pt.X, pt.Y));
                if (!double.IsNaN(pt.Y))
                    snap.Add((pt.X, -pt.Z));
            }

            return new(pts.ToArray(), snap.ToArray());
        }
    }
}
