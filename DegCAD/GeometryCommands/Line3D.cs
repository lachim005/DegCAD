using DegCAD.Dialogs;
using DegCAD.MongeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class Line3D : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Průměty přímky";

            ParametricLine2 line1 = new();
            int line1Sign = 1;
            ParametricLine2 line2 = new();
            int line2Sign = 1;

            //First projection
            esb.CommandHelp = "Vyberte první bod prvního průmětu, pravým tlačítkem změníte průmětnu";
            (Vector2 pt1, bool plane) = await inputMgr.GetPointWithPlane((p, gd, plane) =>
            {
                gd.DrawPlane(plane);
                
                gd.DrawPointCross(p, Style.Default);
            });
            esb.CommandHelp = "Vyberte druhý bod prvního průmětu, pravým tlačítkem změníte průmětnu";
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

            //Second projection
            esb.CommandHelp = "Vyberte první bod druhého průmětu";
            Vector2 pt3 = await inputMgr.GetPoint((p, gd) =>
            {
                gd.DrawPlane(!plane);

                gd.DrawPointCross(pt1, Style.Default);
                gd.DrawPointCross(pt2, Style.Default);
                gd.DrawPointCross(p, Style.Default);

                gd.DrawLine(line1, double.PositiveInfinity * line1Sign, line1.GetParamFromY(0), Style.Default);
            }, lines: new ParametricLine2[1] {line1});
            esb.CommandHelp = "Vyberte druhý bod druhého průmětu";
            Vector2 pt4 = await inputMgr.GetPoint((p, gd) =>
            {
                gd.DrawPlane(!plane);

                gd.DrawPointCross(pt1, Style.Default);
                gd.DrawPointCross(pt2, Style.Default);
                gd.DrawPointCross(pt3, Style.Default);
                gd.DrawPointCross(p, Style.Default);

                gd.DrawLine(line1, double.PositiveInfinity * line1Sign, line1.GetParamFromY(0), Style.Default);

                line2 = ParametricLine2.From2Points(pt3, p);

                //Calculates the infinity sign
                line2Sign = !plane ? -1 : 1;
                if (line2.DirectionVector.Y * line2.DirectionVector.X < 0)
                {
                    line2.DirectionVector = -line2.DirectionVector;
                    line2Sign *= -1;
                }

                gd.DrawLine(line2, double.PositiveInfinity * line2Sign, line2.GetParamFromY(0), Style.Default);
            }, lines: new ParametricLine2[1] { line1 });

            var curStyle = inputMgr.StyleSelector.CurrentStyle;

            List<IMongeItem> mItems = new()
            {
                new LineProjection(line1, plane, curStyle),
                new LineProjection(line2, !plane, curStyle)
            };

            esb.CommandHelp = "Zadejte název přímky";
            LabelInput lid = new();
            lid.subscriptTbx.IsEnabled = false;
            lid.ShowDialog();
            if (!lid.Canceled)
            {
                mItems.Add(new Label(lid.LabelText, plane ? "2" : "1", lid.Superscript, (pt2 + pt1) / 2, curStyle, mItems[0])); 
                mItems.Add(new Label(lid.LabelText, !plane ? "2" : "1", lid.Superscript, (pt3 + pt4) / 2, curStyle, mItems[1]));
            }

            return new TimelineItem(mItems.ToArray());
        }
    }
}
