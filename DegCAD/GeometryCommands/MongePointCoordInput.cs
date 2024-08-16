using DegCAD.Dialogs;
using DegCAD.TimelineElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    internal class MongePointCoordInput : IGeometryCommand
    {
        public Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Vynést body";
            esb.CommandHelp = "Zadejte souřadnice a názvy bodů, které chcete vynést";

            //The dialog is used to enter the point coordinates
            var dialog = new PointCoordinateInputDialog(System.Windows.Window.GetWindow(vpl.Viewport));
            dialog.ShowDialog();

            if (dialog.Canceled)
            {
                throw new CommandCanceledException();
            }

            List<GeometryElement> pts = new();

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            foreach (var pt in dialog.Points)
            {
                //Adds the Y projection
                if (double.IsFinite(pt.Y))
                {
                    pts.Add(new Point(pt.X, pt.Y, curStyle, vpl));
                    //Adds the label
                    if (!string.IsNullOrWhiteSpace(pt.Label))
                    {
                        pts.Add(new Label(pt.Label, "1", "", (pt.X, pt.Y), curStyle, new Point(pt.X, pt.Y), vpl));
                    }
                }
                //Adds the Z projection
                if (double.IsFinite(pt.Z))
                {
                    pts.Add(new Point(pt.X, -pt.Z, curStyle, vpl));
                    //Adds the label
                    if (!string.IsNullOrWhiteSpace(pt.Label))
                    {
                        pts.Add(new Label(pt.Label, "2", "", (pt.X, -pt.Z), curStyle, new Point(pt.X, -pt.Z), vpl));
                    }
                }
            }

            return Task.FromResult<TimelineItem?>(new([.. pts]));
        }
    }
}
