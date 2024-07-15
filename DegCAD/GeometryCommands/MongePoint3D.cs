using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DegCAD.MongeItems;
using System.Diagnostics;
using DegCAD.Dialogs;
using System.Windows.Shapes;

namespace DegCAD.GeometryCommands
{
    public class MongePoint3D : IGeometryCommand
    {
        public Vector2 p1;
        public Vector2 p2;
        bool firstPlane;

        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Průmety bodu";
            
            esb.CommandHelp = "Vyberte první průmět bodu, pravým tlačítkem myši změníte průmětnu";

            Point ptProj1 = new(0, 0, previewVpl);
            Plane plane = new(false, bgVpl);

            ParametricLine2 xLine = new((0, 0), (0, 1));
            Line xLineShape = new();
            xLineShape.SetStyle(Style.BlueDashStyle);
            previewVpl.Canvas.Children.Add(xLineShape);

            (p1, firstPlane) = await inputMgr.GetPointWithPlane((p, pl) =>
            {
                ptProj1.Coords = p;
                ptProj1.Draw();

                xLine.Point = p;
                xLineShape.SetParaLine(previewVpl, xLine, double.NegativeInfinity, double.PositiveInfinity);
                plane.TopPlane = pl;
                plane.Draw();
            });

            esb.CommandHelp = "Vyberte druhý průmět bodu";

            plane.TopPlane = !firstPlane;
            plane.Draw();
            Point ptProj2 = new(p1.X, 0, previewVpl);

            p2 = await inputMgr.GetPoint((p) =>
            {
                ptProj2.Y = p.Y;
                ptProj2.Draw();
                ptProj1.Draw();
                plane.Draw();
                xLineShape.SetParaLine(previewVpl, xLine, double.NegativeInfinity, double.PositiveInfinity);
            }, lines: new ParametricLine2[1] { new(p1, (0, 1)) });

            p2.X = p1.X;

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            Point[] mpoints = new Point[2];
            //Sets the Y and Z coordinates depending on the first plane selected
            if (firstPlane)
            {
                mpoints[0] = new(p1.X, p2.Y, curStyle, vpl);
                mpoints[1] = new(p1.X, p1.Y, curStyle, vpl);
            } else
            {
                mpoints[0] = new(p1.X, p1.Y, curStyle, vpl);
                mpoints[1] = new(p1.X, p2.Y, curStyle, vpl);
            }

            List<IMongeItem> mongeItems = new(3);
            mongeItems.AddRange(mpoints);

            if (inputMgr.NameNewItems)
            {
                esb.CommandHelp = "Zadejte název bodu";
                //Shows the dialog to input the point name
                var lid = new LabelInput();
                lid.subscriptTbx.IsEnabled = false;
                lid.ShowDialog();
                //Adds the labels to the timeline item
                if (!lid.Canceled)
                {
                    mongeItems.Add(new Label(lid.LabelText, "1", lid.Superscript,
                        mpoints[0].Coords, curStyle, mpoints[0].Clone(), vpl, lid.TextSize));
                    mongeItems.Add(new Label(lid.LabelText, "2", lid.Superscript,
                        mpoints[1].Coords, curStyle, mpoints[1].Clone(), vpl, lid.TextSize));
                } 
            }

            return new(mongeItems.ToArray());
        }
    }
}
