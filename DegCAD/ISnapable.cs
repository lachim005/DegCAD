using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public interface ISnapable
    {
        public Vector2[] SnapablePoints { get; }
        public ParametricSegment2[] SnapableLines { get; }
        public Circle2[] SnapableCircles { get; }
        public bool IsVisible { get; }
    }
}
