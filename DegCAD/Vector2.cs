using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD
{
    /// <summary>
    /// A struct for representing a two-dimensional vector or a point
    /// </summary>
    public struct Vector2
    {
        /// <summary>
        /// The X coordinate of the vector
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// The Y coordinate of the vector
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The length of the vector
        /// </summary>
        double Length { get => Math.Sqrt(X * X + Y * Y); }
        /// <summary>
        /// The squared length of the vector (more efficient to calculate)
        /// </summary>
        double LengthSquared { get => X * X + Y * Y; }

        /// <summary>
        /// Creates a new two-dimensional vector from coordinates
        /// </summary>
        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <returns>a string representation of the vector in the (x,y) format</returns>
        public override string ToString()
        {
            return $"({X}; {Y})";
        }

        #region Operators
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2 operator *(Vector2 v, double d) => (v.X * d, v.Y * d);
        public static Vector2 operator /(Vector2 v, double d) => (v.X / d, v.Y / d);

        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.X, -v.Y);
        }

        public static implicit operator Vector2((double, double) tup) => new(tup.Item1, tup.Item2);
        public static implicit operator Vector2(Point p) => new(p.X, p.Y);
        #endregion
    }
}
