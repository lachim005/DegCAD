using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class InputAngle : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Nanést úhel";

            esb.CommandHelp = "Zadejte velikost úhlu, který chcete nanést";

            var angleDeg = InputBox.InputDouble("°", "Zadejte úhel");
            if (angleDeg is null) throw new CommandCanceledException();
            var angleRad = angleDeg / 180 * Math.PI;

            esb.CommandHelp = "Vyberte vrchol úhlu, který chcete nanést";
            vpl.Viewport.Focus();

            Point mVertex = new(0, 0, Style.Default, previewVpl);
       
            Vector2 vertex = await inputMgr.GetPoint((pt) =>
            {
                mVertex.Coords = pt;
                mVertex.Draw();
            });

            esb.CommandHelp = "Vyberte bod udávající rameno úhlu, pravým tlačítkem změníte směr úhlu";

            MongeItems.LineSegment mRay1 = new(vertex, vertex, Style.BlueDashStyle, previewVpl);
            MongeItems.LineSegment mRay2 = new(vertex, vertex, Style.BlueDashStyle, previewVpl);
            Point mResult = new(0, 0, Style.HighlightStyle, previewVpl);

            (var pt, var dir) = await inputMgr.GetPointWithPlane((pt, dir) =>
            {
                mVertex.Draw();

                var ray1 = pt - vertex;
                var ray1Angle = Math.Atan(ray1.Y / ray1.X);

                if (ray1.X < 0)
                {
                    ray1Angle += Math.PI;
                }

                var ray2Angle = ray1Angle + (angleRad.Value * (dir ? 1 : -1));
                var sincos = Math.SinCos(ray2Angle);
                var ray2 = new Vector2(sincos.Cos, sincos.Sin).ChangeLength(ray1.Length);

                mRay1.P2 = pt;
                mRay1.Draw();
                mRay2.P2 = vertex + ray2;
                mRay2.Draw();
                mResult.Coords = vertex + ray2;
                mResult.Draw();
            },
            lines: [new(vertex, (1, 0)), new(vertex, (0, 1))], // Snapping vertical and horizontal lines
            predicate: (pt) => pt != vertex);

            var ray1 = pt - vertex;
            var ray1Angle = Math.Atan(ray1.Y / ray1.X);

            if (ray1.X < 0)
            {
                ray1Angle += Math.PI;
            }

            var ray2Angle = ray1Angle + (angleRad.Value * (dir ? 1 : -1));
            var sincos = Math.SinCos(ray2Angle);
            var ray2 = new Vector2(sincos.Cos, sincos.Sin).ChangeLength(ray1.Length);
            var res = ray2 + vertex;

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<IMongeItem> mItems = new()
            {
                new Point(res.X, res.Y, curStyle, vpl)
            };
            //Label
            esb.CommandHelp = "Zadejte název bodu";
            LabelInput lid = new();
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, res, curStyle, mItems[0].Clone(), vpl, lid.TextSize));
            }

            return new(mItems.ToArray());
        }
    }
}
