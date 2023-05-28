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

            esb.CommandHelp = "Vyberte ohnisko paraboly";

            Point mfocusPt = new(0, 0, previewVpl);

            Vector2 focusPt = await inputMgr.GetPoint((pt) =>
            {
                mfocusPt.Coords = pt;
                mfocusPt.Draw();
            });

            esb.CommandHelp = "Vyberte vrchol paraboly";

            Point mVertexPt = new(0, 0, previewVpl);

            Vector2 vertexPt = await inputMgr.GetPoint((pt) =>
            {
                mVertexPt.Coords = pt;
                mVertexPt.Draw();
                mfocusPt.Draw();
            });

            esb.CommandHelp = "Vyberte konec paraboly";

            Point mEndPt = new(0, 0, previewVpl);
            MongeItems.Parabola mParabola = new(focusPt, vertexPt, vertexPt, Style.HighlightStyle, previewVpl);


            Vector2 endPt = await inputMgr.GetPoint((pt) =>
            {
                mEndPt.Coords = pt;
                mEndPt.Draw();
                mVertexPt.Draw();
                mfocusPt.Draw();

                mParabola.End = pt;
                mParabola.Draw();
            });

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            return new(new IMongeItem[1] { new MongeItems.Parabola(focusPt, vertexPt, endPt, curStyle, vpl) });
        }
    }
}
