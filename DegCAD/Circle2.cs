using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    /// <summary>
    /// Represents a circle defined by it's center and radius
    /// </summary>
    public struct Circle2
    {
        private double _radius;

        public Vector2 Center { get; set; }
        public double Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                RadiusSquared = _radius * _radius;
            }
        }
        public double RadiusSquared { get; private set; }

        public Circle2(Vector2 center, double radius)
        {
            Center = center;
            _radius = radius;
            RadiusSquared = radius * radius;
        }

        public Circle2(Vector2 center, Vector2 pointOnCircle)
        {
            Center = center;
            Vector2 radius = (center - pointOnCircle);
            RadiusSquared = radius.LengthSquared;
            _radius = radius.Length;
        }

        /// <summary>
        /// Returns the intersections of the line and the circle if they intersect
        /// </summary>
        public (Vector2, Vector2)? FindIntersections(ParametricLine2 line)
        {
            var p = line.GetClosestPoint(Center);
            var vector = p - Center;

            //Line doesn't pass through circle
            if (vector.LengthSquared >= RadiusSquared) return null;

            double segmentLength = Math.Sqrt(RadiusSquared - vector.LengthSquared);
            var segmentVector = line.DirectionVector.ChangeLength(segmentLength);

            var pt1 = p + segmentVector;
            var pt2 = p - segmentVector;
            return (pt1, pt2);
        }
        /// <summary>
        /// Returns the intersections of the two circles if they intersect
        /// </summary>
        public (Vector2, Vector2)? FindIntersections(Circle2 circle)
        {
            var centersVector = circle.Center - Center;
            var centerDistance = centersVector.Length;
            //Circles aren't touching
            if (Radius + circle.Radius <= centerDistance) return null;
            //Circle are inside of each other
            if (Math.Abs(Radius - circle.Radius) >= centerDistance) return null;

            var a = (RadiusSquared - circle.RadiusSquared + (centerDistance*centerDistance)) / (2 * centerDistance);
            var h = Math.Sqrt(RadiusSquared - (a * a));

            var m = Center + (centersVector * (a / centerDistance));
            var hVector = new Vector2(centersVector.Y, -centersVector.X).ChangeLength(h);

            var pt1 = m + hVector;
            var pt2 = m - hVector;
            return (pt1, pt2);
        }
        /// <summary>
        /// Returns a point on the circle with the same angle as the given point
        /// </summary>
        public Vector2 TranslatePointToCircle(Vector2 point)
        {
            return Center + (point - Center).ChangeLength(Radius);
        }
        /// <summary>
        /// Returns a point on the circle with the given angle
        /// </summary>
        public Vector2 CalculatePointWithAngle(double angle)
        {
            Vector2 unitVector = Math.SinCos(angle);
            unitVector *= Radius;
            return Center + (unitVector.Y, unitVector.X);
        }
    }
}
