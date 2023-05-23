using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class Line2D : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Průmět přímky";

            esb.CommandHelp = "Vyberte první bod průmětu, pravým tlačítkem změníte průmětnu";

            Plane mPlane = new(false, bgVpl);
            Point mPt1 = new(0, 0, previewVpl);

            (Vector2 pt1, bool plane) = await inputMgr.GetPointWithPlane((p, plane) =>
            {
                mPlane.TopPlane = plane;
                mPlane.Draw();
                mPt1.Coords = p;
                mPt1.Draw();
            });

            esb.CommandHelp = "Vyberte druhý bod průmětu, pravým tlačítkem změníte průmětnu";

            Point mPt2 = new(0, 0, previewVpl);
            ParametricLine2 line1 = new((0, 0), (1, 1));
            LineProjection mLine = new(line1, plane, Style.HighlightStyle, previewVpl);

            (Vector2 pt2, plane) = await inputMgr.GetPointWithPlane((p, plane) =>
            {
                mPlane.TopPlane = plane;
                mPlane.Draw();
                mPt2.Coords = p;
                mPt2.Draw();
                mPt1.Draw();

                line1 = ParametricLine2.From2Points(pt1, p);
                mLine.Line = line1;
                mLine.Plane = plane;
                mLine.Draw();
            }, plane, predicate: (pt) => pt != pt1);


            var curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<IMongeItem> mItems = new()
            {
                new LineProjection(line1, plane, curStyle, vpl)
            };

            esb.CommandHelp = "Zadejte název přímky";
            LabelInput lid = new();
            lid.subscriptTbx.Text = plane ? "2" : "1";
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, (pt2 + pt1) / 2, curStyle, mItems[0].Clone(), vpl));
            }


            return new TimelineItem(mItems.ToArray());
        }
    }
}
