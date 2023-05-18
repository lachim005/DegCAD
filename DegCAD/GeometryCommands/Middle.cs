using DegCAD.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DegCAD.GeometryCommands
{
    public class Middle : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Střed";
            Style redStyle = new() { Color = Colors.Red };

            esb.CommandHelp = "Vyberte první bod";
            Vector2 pt1 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt, Style.Default);
            });
            esb.CommandHelp = "Vyberte druhý bod";
            Vector2 pt2 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt1, Style.Default);
                gd.DrawPointCross(pt, Style.Default);

                gd.DrawPointCross((pt1 + pt) / 2, redStyle);
            });

            var middle = (pt1 + pt2) / 2;

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<IMongeItem> mItems = new()
            {
                new DrawableItems.Point(middle.X, middle.Y, curStyle)
            };
            //Label
            esb.CommandHelp = "Zadejte název bodu";
            LabelInput lid = new();
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new MongeItems.Label(lid.LabelText, lid.Subscript, lid.Superscript, middle, curStyle, mItems[0]));
            }

            return new(mItems.ToArray());
        }
    }
}
