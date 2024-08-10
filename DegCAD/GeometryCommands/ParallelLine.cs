using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Numerics;
using DegCAD.MongeItems;
using DegCAD.Dialogs;
using System.Security.Policy;
using System.Windows.Shapes;
using Plane = DegCAD.MongeItems.Plane;
using System.Windows;
using Point = DegCAD.MongeItems.Point;

namespace DegCAD.GeometryCommands
{
    internal class ParallelLine : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Rovnoběžka";

            esb.CommandHelp = "Vyberte přímku, ke které chcete sestrojit rovnoběžku";

            Line selectedLine = new();
            selectedLine.SetStyle(Style.HighlightStyle);
            previewVpl.Canvas.Children.Add(selectedLine);

            ParametricLine2 line = await inputMgr.GetLine((p, l) =>
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

            esb.CommandHelp = "Vyberte bod, kterým bude rovnoběžka procházet, pravým tlačítkem myši změníte průmětnu";

            previewVpl.Canvas.Children.Remove(selectedLine);
            LineProjection mLineProjection = new(line, false, Style.HighlightStyle, previewVpl);
            Plane mPlane = new(false, bgVpl);
            Point mLinePoint = new(0, 0, previewVpl);

            (Vector2 point, bool plane) = await inputMgr.GetPointWithPlane((pt, plane) =>
            {
                mPlane.TopPlane = plane;
                mPlane.Draw();

                mLinePoint.Coords = pt;
                mLinePoint.Draw();

                line.Point = pt;
                mLineProjection.Line = line;
                mLineProjection.Plane = plane;
                mLineProjection.Draw();
            });

            var curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<GeometryElement> mItems =
            [
                new LineProjection(line, plane, curStyle, vpl)
            ];

            if (inputMgr.NameNewItems)
            {
                esb.CommandHelp = "Zadejte název přímky";
                LabelInput lid = new();
                lid.subscriptTbx.Text = plane ? "2" : "1";
                lid.ShowDialog();
                if (!lid.Canceled)
                {
                    mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, line.Point + line.DirectionVector.ChangeLength(2), curStyle, mItems[0].CloneElement(), vpl, lid.TextSize));
                } 
            }

            return new([.. mItems]);
        }
    }
}
