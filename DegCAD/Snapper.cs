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

        public Vector2 Snap(Vector2 v)
        {
            #region Snap to points
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

            
            if (closestPoint is null)
                return v;
            //If the closest point is within a threshold, returns the snapped point
            if (closestPointDistance < SnapThreshold)
                return (Vector2)closestPoint;


            #endregion
            return v;
        }
    }
}
