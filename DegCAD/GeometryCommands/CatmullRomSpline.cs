using DegCAD.TimelineElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.GeometryCommands
{
    public class CatmullRomSpline : IGeometryCommand
    {
        public async Task<TimelineItem?> ExecuteAsync(ViewportLayer previewVpl, ViewportLayer vpl, ViewportLayer bgVpl, GeometryInputManager inputMgr, EditorStatusBar esb)
        {
            esb.CommandName = "Křivka body";

            esb.CommandHelp = "Vyberte bod údávající směr prvního konce křivky";

            

            List<Point> points = [new(0, 0, previewVpl)];

            List<Vector2> splinePoints = [];

            Vector2 p1 = await inputMgr.GetPoint((pt) =>
            {
                points[^1].Coords = pt;
                foreach (Point p in points)
                {
                    p.Draw();
                }
            });
            splinePoints.Add(p1);
            points.Add(new(0, 0, previewVpl));


            TimelineElements.LineSegment startSeg = new(p1, (0, 0), Style.BlueDashStyle, previewVpl);
            esb.CommandHelp = "Vyberte první bod křivky";

            Vector2 p2 = await inputMgr.GetPoint((pt) =>
            {
                points[^1].Coords = pt;
                startSeg.P2 = pt;
                startSeg.Draw();
                foreach (Point p in points)
                {
                    p.Draw();
                }
            });
            points.Add(new(0, 0, previewVpl));

            TimelineElements.LineSegment endSeg = new(p2, (0, 0), Style.BlueDashStyle, previewVpl);
            esb.CommandHelp = "Vyberte druhý bod křivky";
            splinePoints.Add(p2);
            Vector2 p3 = await inputMgr.GetPoint((pt) =>
            {
                points[^1].Coords = pt;
                startSeg.Draw();
                endSeg.P2 = pt;
                endSeg.Draw();
                foreach (Point p in points)
                {
                    p.Draw();
                }
            });
            splinePoints.Add(p3);

            esb.CommandHelp = "Vyberte třetí bod křivky";

            CRSpline newSplineSegment = new(Style.GreenStyle, previewVpl, p1, p2, p3, (0, 0));
            endSeg.P1 = p3;

            Vector2 p4 = await inputMgr.GetPoint((pt) =>
            {
                points[^1].Coords = pt;
                startSeg.Draw();
                endSeg.P2 = pt;
                endSeg.Draw();

                newSplineSegment.Points[^1] = pt;
                newSplineSegment.CalculatePoints();
                newSplineSegment.Draw();

                foreach (Point p in points)
                {
                    p.Draw();
                }
            });
            splinePoints.Add(p4);

            esb.CommandHelp = "Vyberte další body, jakmile budete mít všechny body, zmáčkněte ESC nebo tlačítko pro zrušení příkazu";

            CRSpline resultSpline = new(Style.HighlightStyle, previewVpl, p1, p2, p3, p4);

            try
            {
                while (true)
                {
                    points.Add(new(0, 0, previewVpl));
                    endSeg.P1 = splinePoints[^1];

                    newSplineSegment.Points.RemoveAt(0);
                    newSplineSegment.Points.Add((0,0));

                    Vector2 pt = await inputMgr.GetPoint((pt) =>
                    {
                        points[^1].Coords = pt;
                        newSplineSegment.Points[^1] = pt;
                        newSplineSegment.CalculatePoints();
                        newSplineSegment.Draw();
                        resultSpline.Draw();

                        startSeg.Draw();
                        endSeg.P2 = pt;
                        endSeg.Draw();

                        foreach (Point p in points)
                        {
                            p.Draw();
                        }
                    });
                    splinePoints.Add(pt);
                    resultSpline.Points.Add(pt);
                    resultSpline.CalculatePoints();
                }
            }
            catch (Exception ex)
            {
                if (ex is not CommandCanceledException) throw;
            }

          

            return new TimelineItem([new CRSpline(inputMgr.StyleSelector.CurrentStyle, vpl, [.. splinePoints])]);
        }
    }
}
