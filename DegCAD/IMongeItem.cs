using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public interface IMongeItem
    {
        void Draw(GeometryDrawer gd);
        public Vector2[] SnapablePoints { get; }
        public ParametricLine2[] SnapableLines { get; }
        public Circle2[] SnapableCircles { get; }
        public Style Style { get; }
    }
}
