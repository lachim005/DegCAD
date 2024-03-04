using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public class AxonometryAxes
    {
        public Vector2 XAxis { get; init; }
        public Vector2 YAxis { get; init; }
        public Vector2 ZAxis { get; init; }

        public AxonometryAxes(Vector2 xAxis, Vector2 yAxis, Vector2 zAxis)
        {
            XAxis = xAxis;
            YAxis = yAxis;
            ZAxis = zAxis;
        }

        public AxonometryAxes Clone()
        {
            return new(XAxis, YAxis, ZAxis);
        }
    }
}
