using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DegCAD.GeometryCommands
{
    public class TranslateSize : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Přenést vzdálenost";

            esb.CommandHelp = "Vyberte první bod vzdálenosti, kterou chcete přenést";

            Point mPt1 = new(0, 0, previewVpl);

            Vector2 pt1 = await inputMgr.GetPoint((pt, gd) =>
            {
                mPt1.Coords = pt;
                mPt1.Draw(previewVpl);
            });

            esb.CommandHelp = "Vyberte druhý bod vzdálenosti, kterou chcete přenést";

            Point mPt2 = new(0, 0, previewVpl);
            MongeItems.LineSegment mSelSeg = new(pt1, pt1, Style.HighlightStyle, previewVpl);

            Vector2 pt2 = await inputMgr.GetPoint((pt, gd) =>
            {
                mPt1.Draw(previewVpl);

                mPt2.Coords = pt;
                mPt2.Draw(previewVpl);

                mSelSeg.P2 = pt;
                mSelSeg.Draw(previewVpl);
            });

            var distance = pt2 - pt1;

            esb.CommandHelp = "Vyberte bod, ze kterého chcete vzdálenost vynést";

            Point mPt3 = new(0, 0, previewVpl);
            mSelSeg.Style = Style.GreenStyle;
            var circle = new Circle2(pt2, pt1);
            MongeItems.Circle mCircle = new(circle, Style.BlueDashStyle, previewVpl);

            Vector2 pt3 = await inputMgr.GetPoint((pt, gd) =>
            {
                mPt1.Draw(previewVpl);
                mPt2.Draw(previewVpl);
                mSelSeg.Draw(previewVpl);

                mPt3.Coords = pt;
                mPt3.Draw(previewVpl);

                circle.Center = pt;
                mCircle.Circle2 = circle;
                mCircle.Draw(previewVpl);
            });

            esb.CommandHelp = "Vyberte směr, kterým chcete vzdálenost vynést";

            Point mPt4 = new(0, 0, previewVpl);
            MongeItems.LineSegment mDirSeg = new(pt3, pt3, Style.HighlightStyle, previewVpl);

            Vector2 pt4 = await inputMgr.GetPoint((pt, gd) =>
            {
                mPt1.Draw(previewVpl);
                mPt2.Draw(previewVpl);
                mPt3.Draw(previewVpl);
                mSelSeg.Draw(previewVpl);
                mCircle.Draw(previewVpl);

                pt = circle.TranslatePointToCircle(pt);

                mDirSeg.P2 = pt;
                mDirSeg.Draw(previewVpl);

                mPt4.Coords = pt;
                mPt4.Draw(previewVpl);
            }, predicate: (pt) => pt != pt3, circles: new Circle2[1] { circle });

            var ptOnCircle = circle.TranslatePointToCircle(pt4);

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<IMongeItem> mItems = new()
            {
                new Point(ptOnCircle.X, ptOnCircle.Y, curStyle, vpl)
            };
            //Label
            esb.CommandHelp = "Zadejte název bodu";
            LabelInput lid = new();
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, ptOnCircle, curStyle, mItems[0].Clone(), vpl));
            }

            return new(mItems.ToArray());
        }
    }
}
