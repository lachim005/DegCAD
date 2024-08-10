using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using Plane = DegCAD.MongeItems.Plane;
using Point = DegCAD.MongeItems.Point;

namespace DegCAD.GeometryCommands
{
    public class PerpendicularInfLine : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Kolmice";

            esb.CommandHelp = "Vyberte přímku, ke které chcete sestrojit kolmici";

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

            esb.CommandHelp = "Vyberte bod, kterým bude kolmice procházet";

            //Makes the line perpendicular
            line.DirectionVector = (line.DirectionVector.Y, -line.DirectionVector.X);

            previewVpl.Canvas.Children.Remove(selectedLine);
            InfiniteLine mLine = new(line, Style.HighlightStyle, previewVpl);
            Point mLinePoint = new(0, 0, previewVpl);

            Vector2 point = await inputMgr.GetPoint((pt) =>
            {
                mLinePoint.Coords = pt;
                mLinePoint.Draw();

                line.Point = pt;
                mLine.Line = line;
                mLine.Draw();
            });

            var curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<GeometryElement> mItems =
            [
                new InfiniteLine(line, curStyle, vpl)
            ];

            if (inputMgr.NameNewItems)
            {
                esb.CommandHelp = "Zadejte název přímky";
                LabelInput lid = new();
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
