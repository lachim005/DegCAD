using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class Parabola : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Parabola";

            esb.CommandHelp = "Vyberte vrchol paraboly";

            Point mfocusPt = new(0, 0, previewVpl);

            Vector2 vertexPt = await inputMgr.GetPoint((pt) =>
            {
                mfocusPt.Coords = pt;
                mfocusPt.Draw();
            });

            esb.CommandHelp = "Vyberte ohnisko paraboly";

            Point mVertexPt = new(0, 0, previewVpl);
            MongeItems.Parabola mInfiniteParabola = new(vertexPt, vertexPt, Style.BlueDashStyle, previewVpl);

            Vector2 focusPt = await inputMgr.GetPoint((pt) =>
            {
                mVertexPt.Coords = pt;
                mVertexPt.Draw();
                mfocusPt.Draw();

                mInfiniteParabola.Focus = pt;
                mInfiniteParabola.Draw();
            },
            lines: [new(vertexPt, (1, 0)), new(vertexPt, (0, 1))]); // Snapping vertical and horizontal lines

            esb.CommandHelp = "Vyberte konec paraboly, pravým tlačítkem vyberete konečnou, nebo nekonečnou parabolu";

            Point mEndPt = new(0, 0, previewVpl);
            MongeItems.Parabola mParabola = new(focusPt, vertexPt, vertexPt, Style.HighlightStyle, previewVpl);


            (Vector2 endPt, bool infinite) = await inputMgr.GetPointWithPlane((pt, pl) =>
            {
                mEndPt.Coords = pt;
                mEndPt.Draw();
                mVertexPt.Draw();
                mfocusPt.Draw();
                mInfiniteParabola.Draw();

                mParabola.Infinite = pl;
                mParabola.End = pt;
                mParabola.Draw();
            });

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            if (infinite)
                return new(new IMongeItem[1] { new MongeItems.Parabola(focusPt, vertexPt, curStyle, vpl) });
            return new(new IMongeItem[1] { new MongeItems.Parabola(focusPt, vertexPt, endPt, curStyle, vpl) });
        }
    }
}
