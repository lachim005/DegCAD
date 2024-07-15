using DegCAD.Dialogs;
using DegCAD.MongeItems;
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
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Střed";
            

            esb.CommandHelp = "Vyberte první bod";

            Point mPt1 = new(0, 0, Style.Default, previewVpl);

            Vector2 pt1 = await inputMgr.GetPoint((pt) =>
            {
                mPt1.Coords = pt;
                mPt1.Draw();
            });

            esb.CommandHelp = "Vyberte druhý bod";


            Point mPt2 = new(0, 0, Style.Default, previewVpl);
            Point mPtMid = new(0, 0, Style.HighlightStyle, previewVpl);

            Vector2 pt2 = await inputMgr.GetPoint((pt) =>
            {
                mPt1.Draw();

                mPt2.Coords = pt;
                mPt2.Draw();

                mPtMid.Coords = (pt1 + pt) / 2;
                mPtMid.Draw();
            });

            var middle = (pt1 + pt2) / 2;

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<IMongeItem> mItems = new()
            {
                new Point(middle.X, middle.Y, curStyle, vpl)
            };

            if (inputMgr.NameNewItems)
            {
                //Label
                esb.CommandHelp = "Zadejte název bodu";
                LabelInput lid = new();
                lid.ShowDialog();
                if (!lid.Canceled)
                {
                    mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, middle, curStyle, mItems[0].Clone(), vpl, lid.TextSize));
                } 
            }

            return new(mItems.ToArray());
        }
    }
}
