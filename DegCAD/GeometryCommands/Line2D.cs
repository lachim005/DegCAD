using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class Line2D : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Průmět přímky";

            ParametricLine2 line1 = new();
            int line1Sign = 1;

            //First projection
            esb.CommandHelp = "Vyberte první bod průmětu, pravým tlačítkem změníte průmětnu";
            (Vector2 pt1, bool plane) = await inputMgr.GetPointWithPlane((p, gd, plane) =>
            {
                gd.DrawPlane(plane);

                gd.DrawPointCross(p, Style.Default);
            });
            esb.CommandHelp = "Vyberte druhý bod průmětu, pravým tlačítkem změníte průmětnu";
            (Vector2 pt2, plane) = await inputMgr.GetPointWithPlane((p, gd, plane) =>
            {
                gd.DrawPlane(plane);

                gd.DrawPointCross(pt1, Style.Default);
                gd.DrawPointCross(p, Style.Default);

                line1 = ParametricLine2.From2Points(pt1, p);

                //Calculates the infinity sign
                line1Sign = plane ? -1 : 1;
                if (line1.DirectionVector.Y * line1.DirectionVector.X < 0)
                {
                    line1.DirectionVector = -line1.DirectionVector;
                    line1Sign *= -1;
                }

                gd.DrawLine(line1, double.PositiveInfinity * line1Sign, line1.GetParamFromY(0), Style.Default);
            }, plane);


            var curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<IMongeItem> mItems = new()
            {
                new LineProjection(line1, plane, curStyle)
            };

            esb.CommandHelp = "Zadejte název přímky";
            LabelInput lid = new();
            lid.subscriptTbx.Text = plane ? "2" : "1";
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new Label(lid.LabelText, lid.Subscript, lid.Superscript, (pt2 + pt1) / 2, curStyle, mItems[0]));
            }


            return new TimelineItem(mItems.ToArray());
        }
    }
}
