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
    }
}
