using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public class Snapper
    {
        private const double SnapThreshold = .02;

        public Timeline Timeline { get; init; }

        public Snapper(Timeline timeline)
        {
            Timeline = timeline;
        }

        public Vector2 Snap(Vector2 v, ParametricLine2? forcedLine = null)
        {
            Vector2? closestPoint = null;
            double closestPointDistance = double.MaxValue;

            //Goes through every snapable point and saves the closest one
            foreach (var cmd in Timeline.CommandHistory)
            {
                for (int j = 0; j < cmd.Items.Length; j++)
                {
                    for (int i = 0; i < cmd.Items[j].SnapablePoints.Length; i++)
                    {
                        double distance = (cmd.Items[j].SnapablePoints[i] - v).LengthSquared;
                        if (distance < closestPointDistance)
                        {
                            closestPointDistance = distance;
                            closestPoint = cmd.Items[j].SnapablePoints[i];
                        }
                    } 
                }
            }

            //Goes through every snapable line and saves all the ones that are close
            List<ParametricLine2> closeLines = new();

            foreach(var cmd in Timeline.CommandHistory)
            {
                for (int i = 0; i < cmd.Items.Length; i++)
                {
                    for (int j = 0; j < cmd.Items[i].SnapableLines.Length; j++)
                    {
                        var line = cmd.Items[i].SnapableLines[j];
                        var point = TranslatePointToLine(line, v);
                        double distance = (v - point).LengthSquared;
                        if (distance < SnapThreshold)
                        {
                            closeLines.Add(line);
                        }
                    }
                }
            }

            //Adds the forced line so intersections can be calculated
            if (forcedLine is not null)
                closeLines.Add((ParametricLine2)forcedLine);

            //If there are more close lines, calculates all the intersections
            if (closeLines.Count > 1)
            {
                List<Vector2> intersectionPts = new();

                //calculates all the intersections
                for (int i = 0; i < closeLines.Count; i++)
                {
                    for (int j = 0; j < closeLines.Count; j++)
                    {
                        if (i == j) continue;
                        intersectionPts.Add(closeLines[i].FindIntersection(closeLines[j]));
                    }
                }

                //Saves the closest intersection point if it is closer than any other snapable point
                for (int i = 0; i < intersectionPts.Count; i++)
                {
                    double distance = (v - intersectionPts[i]).LengthSquared;
                    if (distance < closestPointDistance)
                    {
                        closestPoint = intersectionPts[i];
                        closestPointDistance = distance;
                    }
                }
            }

            //If the closest point is within a threshold, returns the snapped point
            if (closestPoint is not null && closestPointDistance < SnapThreshold)
                return (Vector2)closestPoint;

            //Else returns the point snapped to a line
            if (closeLines.Count > 0)
            {
                return TranslatePointToLine(closeLines[0], v);
            }
            
            //Else returns the original point
            return v;
        }

        public ParametricLine2? SelectLine(Vector2 v)
        {
            ParametricLine2? closestLine = null;
            double closestLineDistance = SnapThreshold;

            //Goes through every line and saves the closest one
            foreach(var cmd in Timeline.CommandHistory)
            {
                for (int i = 0; i < cmd.Items.Length; i++)
                {
                    for (int j = 0; j < cmd.Items[i].SnapableLines.Length; j++)
                    {
                        var line = cmd.Items[i].SnapableLines[j];
                        Vector2 point = TranslatePointToLine(line, v);
                        double distance = (v - point).LengthSquared;
                        if (distance < closestLineDistance)
                        {
                            closestLine = line;
                            closestLineDistance = distance;
                        }
                    }
                }
            }
            return closestLine;
        }

        /// <summary>
        /// Calculates a point on a line that is closest to the input point
        /// </summary>
        private Vector2 TranslatePointToLine(ParametricLine2 line, Vector2 pt)
        {
            //Creates a perpendicular line passing through the set point
            var perpLine = new ParametricLine2(pt, (line.DirectionVector.Y, -line.DirectionVector.X));
            //Returns the intersection point of the two lines
            return line.FindIntersection(perpLine);
        }
    }
}
