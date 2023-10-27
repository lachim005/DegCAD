using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DegCAD.GeometryCommands
{
    public class AddPointProjection : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Doplnit průmět";

            esb.CommandHelp = "Vyberte průmět bodu, ke kterému chcete doplnit průmět";

            Point ptProj1 = new(0, 0, previewVpl);

            ParametricLine2 xLine = new((0, 0), (0, 1));
            Line xLineShape = new();
            xLineShape.SetStyle(Style.BlueDashStyle);
            previewVpl.Canvas.Children.Add(xLineShape);

            var p1 = await inputMgr.GetPoint((p) =>
            {
                ptProj1.Coords = p;
                ptProj1.Draw();

                xLine.Point = p;
                xLineShape.SetParaLine(previewVpl, xLine, double.NegativeInfinity, double.PositiveInfinity);
            });

            esb.CommandHelp = "Vyberte druhý průmět bodu, pravým tlačítkem myši změníte průmětnu";

            Point ptProj2 = new(p1.X, 0, previewVpl);
            Plane plane = new(false, bgVpl);

            (var p2, var pl) = await inputMgr.GetPointWithPlane((p, pl) =>
            {
                ptProj2.Y = p.Y;
                ptProj2.Draw();
                plane.TopPlane = pl;
                plane.Draw();

                ptProj1.Draw();
                xLineShape.SetParaLine(previewVpl, xLine, double.NegativeInfinity, double.PositiveInfinity);
            }, defaultPlane: p1.Y > 0 , lines: new ParametricLine2[1] { new(p1, (0, 1)) });

            p2.X = p1.X;

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;


            List<IMongeItem> mongeItems = new(2)
            {
                new Point(p2.X, p2.Y, curStyle, vpl)
            };

            esb.CommandHelp = "Zadejte název bodu";
            //Shows the dialog to input the point name
            var lid = new LabelInput();
            lid.subscriptTbx.Text = pl ? "2" : "1";
            lid.ShowDialog();
            //Adds the labels to the timeline item
            if (!lid.Canceled)
            {
                mongeItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript,
                    p2, curStyle, mongeItems[0].Clone(), vpl, lid.TextSize));
            }

            return new(mongeItems.ToArray());
        }
    }
}
