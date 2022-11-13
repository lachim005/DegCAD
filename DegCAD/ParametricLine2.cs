using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    /// <summary>
    /// A two-dimensional parametric line represented by a point and a directional vector
    /// </summary>
    public struct ParametricLine2
    {
        /// <summary>
        /// A point contained on the line
        /// </summary>
        public Vector2 Point { get; set; }
        /// <summary>
        /// The directional vector parallel to the line
        /// </summary>
        public Vector2 DirectionVector { get; set; }

        /// <summary>
        /// Creates a new parametric line from a point and a directional vector
        /// </summary>
        public ParametricLine2(Vector2 point, Vector2 directionVector)
        {
            Point = point;
            DirectionVector = directionVector;
        }

        #region Coordinate calulations
        /// <summary>
        /// Returns the parameter corresponding to the X coordinate
        /// </summary>
        public double GetParamFromX(double x)
        {
            return (x - Point.X) / DirectionVector.X;
        }
        /// <summary>
        /// Returns the parameter corresponding to the Y coordinate
        /// </summary>
        public double GetParamFromY(double y)
        {
            return (y - Point.Y) / DirectionVector.Y;
        }
        /// <summary>
        /// Returns the X coordinate corresponding to the parameter
        /// </summary>
        public double GetX(double t)
        {
            return Point.X + (DirectionVector.X * t);
        }
        /// <summary>
        /// Returns the Y coordinate corresponding to the parameter
        /// </summary>
        public double GetY(double t)
        {
            return Point.Y + (DirectionVector.Y * t);
        }
        /// <summary>
        /// Returns the X coordinate to the corresponding Y coordinate on the line
        /// </summary>
        public double CalcX(double y)
        {
            return GetX(GetParamFromY(y));
        }
        /// <summary>
        /// Returns the Y coordinate to the corresponding X coordinate on the line
        /// </summary>
        public double CalcY(double x)
        {
            return GetY(GetParamFromX(x));
        }
        /// <summary>
        /// Returns a point on the line corresponding to the parameter
        /// </summary>
        public Vector2 GetPoint(double t)
        {
            return (GetX(t), GetY(t));
        }
        #endregion
        /// <summary>
        /// Finds and returns an intersection point of two lines
        /// </summary>
        /// <returns>Coordinates or infinity if the lines are parallel or NaN if the lines are the same</returns>
        public Vector2 FindIntersection(ParametricLine2 l)
        {
            //Do not touch, it works
            //It solves:
            //x1 = x2
            //y1 = y2
            //where it finds one of the parameters and uses it to calculate the coordinate

            double a = (l.Point.Y * DirectionVector.X) + (Point.X * DirectionVector.Y) - (DirectionVector.Y * l.Point.X) - (Point.Y * DirectionVector.X);
            double b = (DirectionVector.Y * l.DirectionVector.X) - (l.DirectionVector.Y * DirectionVector.X);
            double t2 = a / b;

            return l.GetPoint(t2);
        }
        /// <summary>
        /// Finds and returns an intersection point of two lines
        /// </summary>
        /// <returns>Coordinates or infinity if the lines are parallel or NaN if the lines are the same</returns>
        public static Vector2 FindIntersection(ParametricLine2 l1, ParametricLine2 l2)
        {
            return l1.FindIntersection(l2);
        }

        /// <returns>a string representation of the line containing the coordinates of the point and the direction vector</returns>
        public override string ToString()
        {
            return $"p: {Point}, dv: {DirectionVector}";
        }

        /// <summary>
        /// Creates a new parametric line going through two points
        /// </summary>
        public static ParametricLine2 From2Points(Vector2 p1, Vector2 p2)
        {
            return new ParametricLine2(p1, p2-p1);
        }
    }
}
