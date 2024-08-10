using DegCAD.Dialogs;
using DegCAD.TimelineElements;
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

            List<GeometryElement> mongeItems = new(2)
            {
                new Point(pt.X, pt.Y, inputMgr.StyleSelector.CurrentStyle, vpl)
            };

            if (inputMgr.NameNewItems)
            {
                esb.CommandHelp = "Zadejte název bodu";
                var lid = new LabelInput();
                lid.ShowDialog();

                if (!lid.Canceled)
                {
                    mongeItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, pt, inputMgr.StyleSelector.CurrentStyle, mongeItems[0].CloneElement(), vpl, lid.TextSize));
                } 
            }

            return new([.. mongeItems]);
        }
    }
}
