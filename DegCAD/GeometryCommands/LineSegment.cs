
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class LineSegment : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Úsečka";

            esb.CommandHelp = "Vyberte počáteční bod úsečky";

            TimelineElements.Point mPt1 = new(0, 0, previewVpl);

            var p1 = await inputMgr.GetPoint((p) =>
            {
                mPt1.Coords = p;
                mPt1.Draw();
            });

            esb.CommandHelp = "Vyberte koncový bod úsečky";

            TimelineElements.Point mPt2 = new(0, 0, previewVpl);
            TimelineElements.LineSegment lineSegment = new(p1, p1, Style.HighlightStyle, previewVpl);

            var p2 = await inputMgr.GetPoint((p) =>
            {
                mPt1.Draw();

                mPt2.Coords = p;
                mPt2.Draw();
                lineSegment.P2 = p;
                lineSegment.Draw();
            },
            lines: [new(p1, (1, 0)), new(p1, (0, 1))]); // Snapping vertical and horizontal lines

            var lseg = new TimelineElements.LineSegment(p1, p2, inputMgr.StyleSelector.CurrentStyle, vpl);

            return new([lseg]);
        }
    }
}
