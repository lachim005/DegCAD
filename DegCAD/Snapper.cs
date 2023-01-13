﻿using System;
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

        public Vector2 Snap(Vector2 v, Vector2[]? points = null, ParametricLine2[]? lines = null, Circle2[]? circles = null)
        {
            Vector2? closestPoint = null;
            double closestPointDistanceSquared = double.MaxValue;

            void SaveClosestPoint(Vector2 pt)
            {
                var distance = (v - pt).LengthSquared;
                if (distance < closestPointDistanceSquared)
                {
                    closestPointDistanceSquared = distance;
                    closestPoint = pt;
                }
            }

            //Goes through every snapable point and saves the closest one
            foreach (var cmd in Timeline.CommandHistory)
            {
                for (int j = 0; j < cmd.Items.Length; j++)
                {
                    for (int i = 0; i < cmd.Items[j].SnapablePoints.Length; i++)
                    {
                        SaveClosestPoint(cmd.Items[j].SnapablePoints[i]);
                    } 
                }
            }

            //Adds the additional points
            if (points is not null)
            {
                foreach (var pt in points)
                {
                    SaveClosestPoint(pt);
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
                        var point = line.GetClosestPoint(v);
                        double distance = (v - point).LengthSquared;
                        if (distance < SnapThreshold)
                        {
                            closeLines.Add(line);
                        }
                    }
                }
            }

            //Adds the additional lines
            if (lines is not null)
            {
                closeLines.AddRange(lines);
            }

            //Goes through every snapable circle and saves all the ones that are close
            List<Circle2> closeCircles = new();
            foreach (var cmd in Timeline.CommandHistory)
            {
                for (int i = 0; i < cmd.Items.Length; i++)
                {
                    for (int j = 0; j < cmd.Items[i].SnapableCircles.Length; j++)
                    {
                        var circle = cmd.Items[i].SnapableCircles[j];
                        var point = circle.TranslatePointToCircle(v);
                        double distance = (v - point).LengthSquared;
                        if (distance < SnapThreshold)
                        {
                            closeCircles.Add(circle);
                        }
                    }
                }
            }

            //Adds the additional circles
            if (circles is not null)
            {
                closeCircles.AddRange(circles);
            }

            //If there are more close lines, calculates all the intersections
            if (closeLines.Count > 1)
            {
                //calculates all the intersections
                for (int i = 0; i < closeLines.Count; i++)
                {
                    for (int j = 0; j < closeLines.Count; j++)
                    {
                        if (i == j) continue;
                        SaveClosestPoint(closeLines[i].FindIntersection(closeLines[j]));
                    }
                }
            }

            //If there is at least one circle and one line, calculates all their intersections
            if (closeLines.Count > 0 && closeCircles.Count > 0)
            {
                foreach(var circle in closeCircles)
                {
                    foreach (var line in closeLines)
                    {
                        var intersections = circle.FindIntersections(line);
                        if (intersections is null) continue;
                        SaveClosestPoint(intersections.Value.Item1);
                        SaveClosestPoint(intersections.Value.Item2);
                    }
                }
            }

            if (closeCircles.Count > 1)
            {
                //calculates all the intersections
                for (int i = 0; i < closeCircles.Count; i++)
                {
                    for (int j = 0; j < closeCircles.Count; j++)
                    {
                        if (i == j) continue;
                        var intersections = closeCircles[i].FindIntersections(closeCircles[j]);
                        if (intersections is null) continue;
                        SaveClosestPoint(intersections.Value.Item1);
                        SaveClosestPoint(intersections.Value.Item2);
                    }
                }
            }

            //If the closest point is within a threshold, returns the snapped point
            if (closestPoint is not null && closestPointDistanceSquared < SnapThreshold)
                return (Vector2)closestPoint;


            //Saves the closest point on a line
            if (closeLines.Count > 0)
            {
                foreach(var line in closeLines)
                {
                    SaveClosestPoint(line.GetClosestPoint(v));
                }
            }

            //Saves the closest point on a circle
            if (closeCircles.Count > 0)
            {
                foreach (var circle in closeCircles)
                {
                    SaveClosestPoint(circle.TranslatePointToCircle(v));
                }
            }

            //If the closest point is within a threshold, returns the snapped point
            if (closestPoint is not null && closestPointDistanceSquared < SnapThreshold)
                return (Vector2)closestPoint;

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
