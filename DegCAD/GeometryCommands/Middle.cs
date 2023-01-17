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
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr)
        {
            Style redStyle = new() { Color = Colors.Red };

            Vector2 pt1 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt, Style.Default);
            });
            Vector2 pt2 = await inputMgr.GetPoint((pt, gd) =>
            {
                gd.DrawPointCross(pt1, Style.Default);
                gd.DrawPointCross(pt, Style.Default);

                gd.DrawPointCross((pt1 + pt) / 2, redStyle);
            });

            var middle = (pt1 + pt2) / 2;

            List<IMongeItem> mItems = new()
            {
                new DrawableItems.Point(middle.X, middle.Y)
            };
            //Label
            LabelInput lid = new();
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new MongeItems.Label(lid.LabelText, lid.Subscript, lid.Superscript, middle, Style.Default, (gd, s) =>
                {
                    gd.DrawPointCross(middle, s);
                }));
            }

            return new(mItems.ToArray());
        }
    }
}
