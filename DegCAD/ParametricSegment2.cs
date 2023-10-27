using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public struct ParametricSegment2
    {
        public ParametricLine2 Line { get; set; }
        public double From { get; set; }
        public double To { get; set; }

        public ParametricSegment2(ParametricLine2 line, double from, double to)
        {
            Line = line;
            From = from;
            To = to;
        }

        public bool IsOnSegment(Vector2 pt)
        {
            double from, to;
            if (From > To)
            {
                from = To;
                to = From;
            } 
            else
            {
                from = From;
                to = To;
            }

            if (Math.Abs(Line.DirectionVector.X) < Math.Abs(Line.DirectionVector.Y))
            {
                var ypar = Line.GetParamFromY(pt.Y);
                return ypar >= from && ypar <= to;
            }
            var xpar = Line.GetParamFromX(pt.X);
            return xpar >= from && xpar <= to;
        }
    }
}
