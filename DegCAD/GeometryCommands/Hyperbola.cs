using DegCAD.TimelineElements;
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
            InfiniteLine mainAxis = new(new((0,0), (1,0)), Style.BlueDashStyle, previewVpl);

            Vector2 vertexPt = await inputMgr.GetPoint((pt) =>
            {
                mVertexPt.Coords = pt;
                mainAxis.Line = ParametricLine2.From2Points(centerPt, pt);
                mainAxis.Draw();
                mVertexPt.Draw();
                mCenterPt.Draw();
            }, predicate: (pt) => pt != centerPt,
            lines: [new(centerPt, (1, 0)), new(centerPt, (0, 1))]); // Snapping vertical and horizontal lines

            esb.CommandHelp = "Vyberte koncový bod hyperboly";

            Point mEndPt = new(0, 0, previewVpl);
            TimelineElements.Hyperbola mHyperbola = new(vertexPt, centerPt, centerPt, Style.HighlightStyle, previewVpl);

            var verCen = vertexPt - centerPt;
            var verCenLine = new ParametricLine2(centerPt, verCen);
            var angle = verCen.Angle;
            var rv = vertexPt.RotateVector(-angle);

            var asymptote1 = new InfiniteLine(new((0, 0), (1, 0)), Style.BlueDashStyle, previewVpl);
            var asymptote2 = new InfiniteLine(new((0, 0), (1, 0)), Style.BlueDashStyle, previewVpl);

            var perpVec = new Vector2(verCen.Y, -verCen.X);

            Vector2 endPt = await inputMgr.GetPoint((pt) =>
            {
                mEndPt.Coords = pt;
                mEndPt.Draw();
                mainAxis.Draw();
                mVertexPt.Draw();
                mCenterPt.Draw();

                mHyperbola.Point = pt;
                mHyperbola.Draw();

                var b = Math.Sqrt(mHyperbola.Eccentricity * mHyperbola.Eccentricity - mHyperbola.A * mHyperbola.A);
                perpVec = perpVec.ChangeLength(b);
                asymptote1.Line = ParametricLine2.From2Points(centerPt, vertexPt + perpVec);
                asymptote2.Line = ParametricLine2.From2Points(centerPt, vertexPt - perpVec);
                asymptote1.Draw();
                asymptote2.Draw();
            }, predicate: (pt) => {
                if ((verCenLine.GetClosestPoint(pt) - pt).LengthSquared < .01) return false;
                if ((pt - vertexPt).LengthSquared < .5) return false;
           
                var rp = pt.RotateVector(-angle);
                if (rp.X <= rv.X) return false;

                return true;
            });

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            return new([new TimelineElements.Hyperbola(vertexPt, centerPt, endPt, curStyle, vpl)]);
        }
    }
}
