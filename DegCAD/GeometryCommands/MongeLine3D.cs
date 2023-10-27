using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class MongeLine3D : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Průměty přímky";

            esb.CommandHelp = "Vyberte první bod prvního průmětu, pravým tlačítkem změníte průmětnu";

            Plane mPlane = new(false, bgVpl);
            Point mPt1 = new(0, 0, previewVpl);

            (Vector2 pt1, bool plane) = await inputMgr.GetPointWithPlane((p, plane) =>
            {
                mPlane.TopPlane = plane;
                mPlane.Draw();
                mPt1.Coords = p;
                mPt1.Draw();
            });

            esb.CommandHelp = "Vyberte druhý bod prvního průmětu, pravým tlačítkem změníte průmětnu";

            Point mPt2 = new(0, 0, previewVpl);
            ParametricLine2 line1 = new((0, 0), (1, 1));
            LineProjection mLine1 = new(line1, plane, Style.HighlightStyle, previewVpl);

            (Vector2 pt2, plane) = await inputMgr.GetPointWithPlane((p, plane) =>
            {
                mPlane.TopPlane = plane;
                mPlane.Draw();
                mPt2.Coords = p;
                mPt2.Draw();
                mPt1.Draw();

                line1 = ParametricLine2.From2Points(pt1, p);
                mLine1.Line = line1;
                mLine1.Plane = plane;
                mLine1.Draw();
            }, plane, predicate: (pt) => pt != pt1);

            esb.CommandHelp = "Vyberte první bod druhého průmětu";

            Point mPt3 = new(0, 0, previewVpl);
            mPlane.TopPlane = !mPlane.TopPlane;

            Vector2 pt3 = await inputMgr.GetPoint((p) =>
            {
                mPlane.Draw();
                mLine1.Draw();

                mPt3.Coords = p;
                mPt3.Draw();
                mPt2.Draw();
                mPt1.Draw();
            }, lines: new ParametricLine2[1] {line1});

            esb.CommandHelp = "Vyberte druhý bod druhého průmětu";

            Point mPt4 = new(0, 0, previewVpl);
            ParametricLine2 line2 = new((0, 0), (1, 1));
            LineProjection mLine2 = new(line1, !plane, Style.HighlightStyle, previewVpl);

            Vector2 pt4 = await inputMgr.GetPoint((p) =>
            {
                mPlane.Draw();
                mLine1.Draw();

                mPt4.Coords = p;
                mPt4.Draw();
                mPt3.Draw();
                mPt2.Draw();
                mPt1.Draw();

                line2 = ParametricLine2.From2Points(pt3, p);
                mLine2.Line = line2;
                mLine2.Draw();
            }, lines: new ParametricLine2[1] { line1 }, predicate: (pt) => pt != pt3);

            var curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<IMongeItem> mItems = new()
            {
                new LineProjection(line1, plane, curStyle, vpl),
                new LineProjection(line2, !plane, curStyle, vpl)
            };

            esb.CommandHelp = "Zadejte název přímky";
            LabelInput lid = new();
            lid.subscriptTbx.IsEnabled = false;
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new Label(lid.LabelText, plane ? "2" : "1", lid.Superscript, (pt2 + pt1) / 2, curStyle, mItems[0].Clone(), vpl, lid.TextSize)); 
                mItems.Add(new Label(lid.LabelText, !plane ? "2" : "1", lid.Superscript, (pt3 + pt4) / 2, curStyle, mItems[1].Clone(), vpl, lid.TextSize));
            }

            return new TimelineItem(mItems.ToArray());
        }
    }
}
