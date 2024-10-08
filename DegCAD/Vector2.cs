﻿using System;
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
        public double Length { get => Math.Sqrt(X * X + Y * Y); }
        /// <summary>
        /// The squared length of the vector (more efficient to calculate)
        /// </summary>
        public double LengthSquared { get => X * X + Y * Y; }
        /// <summary>
        /// Angle of the vector 
        /// </summary>
        public readonly double Angle 
        {
            get
            {
                var angle = Math.Atan2(Y, X);
                if (angle < 0) angle += Math.Tau;
                return angle;
            }
        }

        /// <summary>
        /// Creates a new two-dimensional vector from coordinates
        /// </summary>
        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Returns a new Vector with the same angle but with the set length
        /// </summary>
        public Vector2 ChangeLength(double length)
        {
            double scalingFactor = length / Length;
            return this * scalingFactor;
        }

        public Vector2 RotateVector(double angle)
        {
            var sc = Math.SinCos(angle);
            return (X * sc.Cos - Y * sc.Sin, X * sc.Sin + Y * sc.Cos);
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
        public static bool operator ==(Vector2 v1, Vector2 v2) => v1.X == v2.X && v1.Y == v2.Y;
        public static bool operator !=(Vector2 v1, Vector2 v2) => v1.X != v2.X || v1.Y != v2.Y;

        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.X, -v.Y);
        }

        public static implicit operator Vector2((double, double) tup) => new(tup.Item1, tup.Item2);
        public static implicit operator Vector2(Point p) => new(p.X, p.Y);
        #endregion

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (obj is not Vector2 vec) return false;
            return vec == this;
        }

        public override int GetHashCode()
        {
            return $"{X},{Y}".GetHashCode();
        }
    }
}
