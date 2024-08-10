using DegCAD.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Text;
using System.Windows.Shapes;

namespace DegCAD.GeometryCommands
{
    public class HalfLine : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Polopřímka";

            esb.CommandHelp = "Vyberte počáteční bod polopřímky";

            MongeItems.Point mPt1 = new(0, 0, previewVpl);

            var p1 = await inputMgr.GetPoint((p) =>
            {
                mPt1.Coords = p;
                mPt1.Draw();
            });

            esb.CommandHelp = "Vyberte směr polopřímky, pravým tlačítkem převrátíte směr";

            MongeItems.Point mPt2 = new(0, 0, previewVpl);
            MongeItems.HalfLine halfLine = new(p1, (0,1), Style.HighlightStyle, previewVpl);

            (var p2, var flip) = await inputMgr.GetPointWithPlane((p, flip) =>
            {
                mPt1.Draw();

                mPt2.Coords = p;
                mPt2.Draw();
                halfLine.Direction = p1 - p;
                if (flip) halfLine.Direction *= -1;
                halfLine.Draw();
            },
            lines: [new(p1, (1, 0)), new(p1, (0, 1))], // Snapping vertical and horizontal lines
            predicate: (p) => p != p1);

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;
            List<GeometryElement> mItems = [];
            mItems.Add(new MongeItems.HalfLine(p1, (p1 - p2) * (flip ? -1 : 1), curStyle, vpl));

            if (inputMgr.NameNewItems)
            {
                esb.CommandHelp = "Zadejte název polopřímky";
                LabelInput lid = new();
                lid.ShowDialog();
                if (!lid.Canceled)
                {
                    mItems.Add(new MongeItems.Label(lid.LabelText, lid.Subscript, lid.Superscript, p1, curStyle, mItems[0].CloneElement(), vpl, lid.TextSize));
                } 
            }

            return new([.. mItems]);
        }
    }
}
