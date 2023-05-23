using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Plane = DegCAD.MongeItems.Plane;
using System.Windows;
using System.Windows.Shapes;
using Point = DegCAD.MongeItems.Point;

namespace DegCAD.GeometryCommands
{
    internal class PerpendicularLine : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Kolmice";

            esb.CommandHelp = "Vyberte přímku, ke které chcete sestrojit kolmici";

            Line selectedLine = new();
            selectedLine.SetStyle(Style.HighlightStyle);
            previewVpl.Canvas.Children.Add(selectedLine);

            ParametricLine2 line = await inputMgr.GetLine((p, l, gd) =>
            {
                if (l is not null)
                {
                    selectedLine.SetParaLine(previewVpl, (ParametricLine2)l, double.NegativeInfinity, double.PositiveInfinity);
                    selectedLine.Visibility = Visibility.Visible;
                }
                else
                {
                    selectedLine.Visibility = Visibility.Hidden;
                }
            });

            esb.CommandHelp = "Vyberte bod, kterým bude kolmice procházet, pravým tlačítkem myši změníte průmětnu";

            //Makes the line perpendicular
            line.DirectionVector = (line.DirectionVector.Y, -line.DirectionVector.X);

            previewVpl.Canvas.Children.Remove(selectedLine);
            LineProjection mLineProjection = new(line, false, Style.HighlightStyle, previewVpl);
            Plane mPlane = new(false, bgVpl);
            Point mLinePoint = new(0, 0, previewVpl);

            (Vector2 point, bool plane) = await inputMgr.GetPointWithPlane((pt, gd, plane) =>
            {
                mPlane.TopPlane = plane;
                mPlane.Draw(bgVpl);

                mLinePoint.Coords = pt;
                mLinePoint.Draw(previewVpl);

                line.Point = pt;
                mLineProjection.Line = line;
                mLineProjection.Plane = plane;
                mLineProjection.Draw(previewVpl);
            });

            var curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<IMongeItem> mItems = new()
            {
                new LineProjection(line, plane, curStyle, vpl)
            };

            esb.CommandHelp = "Zadejte název přímky";
            LabelInput lid = new();
            lid.subscriptTbx.Text = plane ? "2" : "1";
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, line.Point + line.DirectionVector.ChangeLength(2), curStyle, mItems[0].Clone(), vpl));
            }

            return new(mItems.ToArray());
        }
    }
}
