using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DegCAD.GeometryCommands
{
    public class AddPointProjection : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Doplnit průmět";
            Style previewStyle = new() { Color = Color.FromRgb(0, 0, 255), LineStyle = 1 };
            ParametricLine2 xLine = new((0, 0), (0, 1));

            esb.CommandHelp = "Vyberte průmět bodu, ke kterému chcete doplnit průmět";
            var p1 = await inputMgr.GetPoint((p, gd) =>
            {
                //X line
                xLine.Point = p;
                gd.DrawLine(xLine, double.NegativeInfinity, double.PositiveInfinity, previewStyle);
                //Point 1 cross
                gd.DrawPointCross(p, Style.Default);
            });

            esb.CommandHelp = "Vyberte druhý průmět bodu, pravým tlačítkem myši změníte průmětnu";
            (var p2, var pl) = await inputMgr.GetPointWithPlane((p, gd, pl) =>
            {
                gd.DrawPlane(pl);
                //X line
                gd.DrawLine(xLine, double.NegativeInfinity, double.PositiveInfinity, previewStyle);
                //Point 1 cross
                gd.DrawPointCross(p1, Style.Default);
                //Point 2 cross
                gd.DrawPointCross((p1.X, p.Y), Style.Default);
            }, defaultPlane: p1.Y > 0 , lines: new ParametricLine2[1] { new(p1, (0, 1)) });

            p2.X = p1.X;

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;


            List<IMongeItem> mongeItems = new(2)
            {
                new Point(p2.X, p2.Y, curStyle)
            };

            esb.CommandHelp = "Zadejte název bodu";
            //Shows the dialog to input the point name
            var lid = new LabelInput();
            lid.subscriptTbx.Text = pl ? "2" : "1";
            lid.ShowDialog();
            //Adds the labels to the timeline item
            if (!lid.Canceled)
            {
                mongeItems.Add(new MongeItems.Label(lid.LabelText, lid.Subscript, lid.Superscript,
                    p2, curStyle, mongeItems[0]));
            }

            return new(mongeItems.ToArray());
        }
    }
}
