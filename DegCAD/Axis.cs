using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    /// <summary>
    /// Static class that can display the x axis and the zero mark
    /// </summary>
    public static class Axis
    {
        static ParametricLine2 axis = new((0, 0), (1, 0));

        public static void Draw(GeometryDrawer gd)
        {
            //Axis
            gd.DrawLine(axis, double.NegativeInfinity, double.PositiveInfinity, Style.Default);
            //Zero mark
            gd.DrawLine((0,-.2),(0,.2), Style.Default);
            gd.DrawString("0", (0.1, 0.1), 24, Style.Default);
            //Axis label
            Vector2 end = gd.DrawString("x", (5, 0.1), 24, Style.Default);
            gd.DrawString("1, 2", (end.X, .45), 16, Style.Default);
        }
    }
}
