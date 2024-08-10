using DegCAD.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class InfLine : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Přímka";

            esb.CommandHelp = "Vyberte první bod přímky";

            TimelineElements.Point mPt1 = new(0, 0, previewVpl);

            var p1 = await inputMgr.GetPoint((p) =>
            {
                mPt1.Coords = p;
                mPt1.Draw();
            });

            esb.CommandHelp = "Vyberte druhý bod přímky";

            TimelineElements.Point mPt2 = new(0, 0, previewVpl);
            TimelineElements.InfiniteLine infLine = new(new(p1, (0, 1)), Style.HighlightStyle, previewVpl);

            var p2 = await inputMgr.GetPoint((p) =>
            {
                mPt1.Draw();

                mPt2.Coords = p;
                mPt2.Draw();
                infLine.Direction = p1 - p;
                infLine.Draw();
            },
            lines: [new(p1, (1, 0)), new(p1, (0, 1))], // Snapping vertical and horizontal lines
            predicate: (p) => p != p1);

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;
            List<GeometryElement> mItems = [];
            mItems.Add(new TimelineElements.InfiniteLine(new(p1, p1 - p2), curStyle, vpl));

            if (inputMgr.NameNewItems)
            {
                esb.CommandHelp = "Zadejte název přímky";
                LabelInput lid = new();
                lid.ShowDialog();
                if (!lid.Canceled)
                {
                    mItems.Add(new TimelineElements.Label(lid.LabelText, lid.Subscript, lid.Superscript, p2, curStyle, mItems[0].CloneElement(), vpl, lid.TextSize));
                } 
            }

            return new([.. mItems]);
        }
    }
}
