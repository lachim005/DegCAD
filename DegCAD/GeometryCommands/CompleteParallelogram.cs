using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using Point = DegCAD.MongeItems.Point;
using DegCAD.Dialogs;
using DegCAD.MongeItems;

namespace DegCAD.GeometryCommands
{
    public class CompleteParallelogram : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Doplnit rovnoběžník";

            esb.CommandHelp = "Vyberte první stranu rovnoběžníku";

            Line selectedLine1 = new();
            selectedLine1.SetStyle(Style.HighlightStyle);
            previewVpl.Canvas.Children.Add(selectedLine1);

            ParametricLine2 line1 = await inputMgr.GetLine((p, l) =>
            {
                if (l is not null)
                {
                    selectedLine1.SetParaLine(previewVpl, (ParametricLine2)l, double.NegativeInfinity, double.PositiveInfinity);
                    selectedLine1.Visibility = Visibility.Visible;
                }
                else
                {
                    selectedLine1.Visibility = Visibility.Hidden;
                }
            });

            esb.CommandHelp = "Vyberte bod, který se nachází na této straně";

            selectedLine1.SetStyle(Style.BlueDashStyle);
            selectedLine1.Visibility = Visibility.Visible;
            Point mLinePoint1 = new(0, 0, previewVpl);

            Vector2 line1Point = await inputMgr.GetPoint((pt) =>
            {
                line1.Point = pt;
                mLinePoint1.Coords = pt;
                mLinePoint1.Draw();
                selectedLine1.SetParaLine(previewVpl, line1, double.NegativeInfinity, double.PositiveInfinity);
            });

            esb.CommandHelp = "Vyberte první stranu rovnoběžníku";

            Line selectedLine2 = new();
            selectedLine2.SetStyle(Style.HighlightStyle);
            previewVpl.Canvas.Children.Add(selectedLine2);

            ParametricLine2 line2 = await inputMgr.GetLine((p, l) =>
            {
                mLinePoint1.Draw();
                selectedLine1.SetParaLine(previewVpl, line1, double.NegativeInfinity, double.PositiveInfinity);
                if (l is not null)
                {
                    selectedLine2.SetParaLine(previewVpl, (ParametricLine2)l, double.NegativeInfinity, double.PositiveInfinity);
                    selectedLine2.Visibility = Visibility.Visible;
                }
                else
                {
                    selectedLine2.Visibility = Visibility.Hidden;
                }
            });

            esb.CommandHelp = "Vyberte bod, který se nachází na této straně";

            selectedLine2.SetStyle(Style.BlueDashStyle);
            selectedLine2.Visibility = Visibility.Visible;
            Point mLinePoint2 = new(0, 0, previewVpl);
            MongeItems.LineSegment lseg1 = new(line1Point, (1, 0), Style.GreenStyle,  previewVpl);
            MongeItems.LineSegment lseg2 = new((0, 0), (1, 0), Style.GreenStyle, previewVpl);
            Point mResPoint = new(0, 0, Style.HighlightStyle, previewVpl);

            ParametricLine2 newLine1 = line2;
            newLine1.Point = line1Point;
            ParametricLine2 newLine2 = line1;
            Vector2 intersection = newLine1.FindIntersection(newLine2);

            if (!double.IsFinite(intersection.X) || !double.IsFinite(intersection.Y))
            {
                throw new Exception("Vybrány rovnoběžné přímky");
            }

            Vector2 line2Point = await inputMgr.GetPoint((pt) =>
            {
                newLine2.Point = pt;
                lseg2.P1 = pt;
                intersection = newLine1.FindIntersection(newLine2);            

                lseg1.P2 = intersection;
                lseg2.P2 = intersection;

                lseg1.Draw();
                lseg2.Draw();

                mResPoint.X = intersection.X;
                mResPoint.Y = intersection.Y;
                mResPoint.Draw();

                line2.Point = pt;
                mLinePoint2.Coords = pt;
                mLinePoint2.Draw();
                selectedLine2.SetParaLine(previewVpl, line2, double.NegativeInfinity, double.PositiveInfinity);

                mLinePoint1.Draw();
                selectedLine1.SetParaLine(previewVpl, line1, double.NegativeInfinity, double.PositiveInfinity);
            });

            var curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<IMongeItem> mItems = new()
            {
                new Point(intersection.X, intersection.Y, curStyle, vpl)
            };

            esb.CommandHelp = "Zadejte název bodu";
            LabelInput lid = new();
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, intersection, curStyle, mItems[0].Clone(), vpl, lid.TextSize));
            }

            return new(mItems.ToArray());
        }
    }
}