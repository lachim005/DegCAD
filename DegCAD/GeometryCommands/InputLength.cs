using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class InputLength : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Nanést vzdálenost";

            esb.CommandHelp = "Zadejte délku, kterou chcete nanést";

            var distance = InputBox.InputDouble(title: "Zadejte délku");
            if (distance is null) throw new CommandCanceledException();

            esb.CommandHelp = "Vyberte bod, ze kterého chcete vzdálenost nanést";
            vpl.Viewport.Focus();

            Point mPt3 = new(0, 0, previewVpl);
            var circle = new Circle2((0,0), distance.Value);
            MongeItems.Circle mCircle = new(circle, Style.BlueDashStyle, previewVpl);

            Vector2 pt3 = await inputMgr.GetPoint((pt) =>
            {
                mPt3.Coords = pt;
                mPt3.Draw();

                circle.Center = pt;
                mCircle.Circle2 = circle;
                mCircle.Draw();
            });

            esb.CommandHelp = "Vyberte směr, kterým chcete vzdálenost nanést";

            Point mPt4 = new(0, 0, previewVpl);
            MongeItems.LineSegment mDirSeg = new(pt3, pt3, Style.HighlightStyle, previewVpl);

            Vector2 pt4 = await inputMgr.GetPoint((pt) =>
            {
                mPt3.Draw();
                mCircle.Draw();

                pt = circle.TranslatePointToCircle(pt);

                mDirSeg.P2 = pt;
                mDirSeg.Draw();

                mPt4.Coords = pt;
                mPt4.Draw();
            },
            lines: [new(pt3, (1, 0)), new(pt3, (0, 1))], // Snapping vertical and horizontal lines
            predicate: (pt) => pt != pt3, circles: new Circle2[1] { circle });

            var ptOnCircle = circle.TranslatePointToCircle(pt4);

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<GeometryElement> mItems =
            [
                new Point(ptOnCircle.X, ptOnCircle.Y, curStyle, vpl)
            ];

            if (inputMgr.NameNewItems)
            {
                //Label
                esb.CommandHelp = "Zadejte název bodu";
                LabelInput lid = new();
                lid.ShowDialog();
                if (!lid.Canceled)
                {
                    mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, ptOnCircle, curStyle, mItems[0].CloneElement(), vpl, lid.TextSize));
                } 
            }

            return new([.. mItems]);
        }
    }
}
