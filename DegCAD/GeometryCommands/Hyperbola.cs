using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    internal class Hyperbola : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Hyperbola";

            esb.CommandHelp = "Vyberte střed hyperboly";

            Point mCenterPt = new(0, 0, previewVpl);

            Vector2 centerPt = await inputMgr.GetPoint((pt) =>
            {
                mCenterPt.Coords = pt;
                mCenterPt.Draw();
            });

            esb.CommandHelp = "Vyberte vrchol větve hyperboly";

            Point mVertexPt = new(0, 0, previewVpl);

            Vector2 vertexPt = await inputMgr.GetPoint((pt) =>
            {
                mVertexPt.Coords = pt;
                mVertexPt.Draw();
                mCenterPt.Draw();
            },
            lines: [new(centerPt, (1, 0)), new(centerPt, (0, 1))]); // Snapping vertical and horizontal lines

            esb.CommandHelp = "Vyberte koncový bod hyperboly";

            Point mEndPt = new(0, 0, previewVpl);
            MongeItems.Hyperbola mHyperbola = new(vertexPt, centerPt, centerPt, Style.HighlightStyle, previewVpl);


            Vector2 endPt = await inputMgr.GetPoint((pt) =>
            {
                mEndPt.Coords = pt;
                mEndPt.Draw();
                mVertexPt.Draw();
                mCenterPt.Draw();

                mHyperbola.Point = pt;
                mHyperbola.Draw();
            });

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            return new([new MongeItems.Hyperbola(vertexPt, centerPt, endPt, curStyle, vpl)]);
        }
    }
}
