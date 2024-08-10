using DegCAD.Dialogs;
using DegCAD.TimelineElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plane = DegCAD.TimelineElements.Plane;
using System.Windows;
using System.Windows.Shapes;
using Point = DegCAD.TimelineElements.Point;

namespace DegCAD.GeometryCommands
{
    public class TranslateOnParallelLine : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Přenést po rovnoběžce";
            esb.CommandHelp = "Vyberte přímku, ke které bude dána rovnoběžka, po které přenesete bod";

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

            esb.CommandHelp = "Vyberte bod, který chcete po rovnoběžce přenést";

            selectedLine.SetStyle(Style.BlueDashStyle);
            selectedLine.Visibility = Visibility.Visible;
            Point mLinePoint = new(0, 0, previewVpl);

            Vector2 linePoint = await inputMgr.GetPoint((pt) =>
            {
                line.Point = pt;
                mLinePoint.Coords = pt;
                mLinePoint.Draw();
                selectedLine.SetParaLine(previewVpl, line, double.NegativeInfinity, double.PositiveInfinity);
            });

            esb.CommandHelp = "Vyberte přenesený bod";

            Point mSegPt1 = new(0, 0, previewVpl);

            var p1 = await inputMgr.GetPoint((pt) =>
            {
                pt = line.GetClosestPoint(pt);
                mLinePoint.Draw();

                mSegPt1.Coords = pt;
                mSegPt1.Draw();
                selectedLine.SetParaLine(previewVpl, line, double.NegativeInfinity, double.PositiveInfinity);
            }, lines: new ParametricLine2[1] { line });

            p1 = line.GetClosestPoint(p1);


            var curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<GeometryElement> mItems =
            [
                new Point(p1.X, p1.Y, curStyle, vpl)
            ];

            if (inputMgr.NameNewItems)
            {
                esb.CommandHelp = "Zadejte název bodu";
                LabelInput lid = new();
                lid.ShowDialog();
                if (!lid.Canceled)
                {
                    mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, p1, curStyle, mItems[0].CloneElement(), vpl, lid.TextSize));
                } 
            }

            return new([.. mItems]);
        }
    }
}
