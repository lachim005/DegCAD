using DegCAD.Dialogs;
using DegCAD.DrawableItems;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class Point2D : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Bod";

            esb.CommandHelp = "Vyberte bod";
            var pt = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt, Style.Default);
            });

            List<IMongeItem> mongeItems = new(2)
            {
                new Point(pt.X, pt.Y, inputMgr.StyleSelector.CurrentStyle)
            };

            esb.CommandHelp = "Zadejte název bodu";
            var lid = new LabelInput();
            lid.ShowDialog();

            if (!lid.Canceled)
            {
                mongeItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, pt, inputMgr.StyleSelector.CurrentStyle, mongeItems[0]));
            }
            return new(mongeItems.ToArray());
        }
    }
}
