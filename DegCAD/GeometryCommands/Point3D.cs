using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DegCAD.DrawableItems;
using System.Diagnostics;
using DegCAD.Dialogs;

namespace DegCAD.GeometryCommands
{
    public class Point3D : IGeometryCommand
    {
        public Vector2 p1;
        public Vector2 p2;
        bool firstPlane;

        public async Task<TimelineItem?> ExecuteAsync(GeometryDrawer gd, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            Style previewStyle = new() { Color = Color.FromRgb(0, 0, 255), LineStyle = 1 };
            ParametricLine2 xLine = new((0, 0), (0, 1));

            (p1, firstPlane) = await inputMgr.GetPointWithPlane((p, gd, pl) =>
            {
                gd.DrawPlane(pl);
                //X line
                xLine.Point = p;
                gd.DrawLine(xLine, double.NegativeInfinity, double.PositiveInfinity, previewStyle);
                //Point 1 cross
                gd.DrawPointCross(p, Style.Default);
            });

            p2 = await inputMgr.GetPoint((p, gd) =>
            {
                gd.DrawPlane(!firstPlane);
                //X line
                gd.DrawLine(xLine, double.NegativeInfinity, double.PositiveInfinity, previewStyle);
                //Point 1 cross
                gd.DrawPointCross(p1, Style.Default);
                //Point 2 cross
                gd.DrawPointCross((p1.X, p.Y), Style.Default);
            }, lines: new ParametricLine2[1] { new(p1, (0, 1)) });

            p2.X = p1.X;

            Style curStyle = inputMgr.StyleSelector.CurrentStyle;

            Point[] mpoints = new Point[2];
            //Sets the Y and Z coordinates depending on the first plane selected
            if (firstPlane)
            {
                mpoints[0] = new(p1.X, p2.Y, curStyle);
                mpoints[1] = new(p1.X, p1.Y, curStyle);
            } else
            {
                mpoints[0] = new(p1.X, p1.Y, curStyle);
                mpoints[1] = new(p1.X, p2.Y, curStyle);
            }

            List<IMongeItem> mongeItems = new(3);
            mongeItems.AddRange(mpoints);

            //Shows the dialog to input the point name
            var lid = new LabelInput();
            lid.subscriptTbx.IsEnabled = false;
            lid.ShowDialog();
            //Adds the labels to the timeline item
            if (!lid.Canceled)
            {
                mongeItems.Add(new MongeItems.Label(lid.LabelText, "1", lid.Superscript,
                    mpoints[0].Coords, curStyle, mpoints[0]));
                mongeItems.Add(new MongeItems.Label(lid.LabelText, "2", lid.Superscript,
                    mpoints[1].Coords, curStyle, mpoints[1]));
            }

            return new(mongeItems.ToArray());
        }
    }
}
