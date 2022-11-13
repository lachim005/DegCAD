using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MongeItems
{
    /// <summary>
    /// Static class that can display the x axis and the zero mark
    /// </summary>
    public class Axis : IMongeItem
    {
        static ParametricLine2 axis = new((0, 0), (1, 0));

        public Vector2[] SnapablePoints { get; } = new Vector2[1] { (0, 0) };

        public ParametricLine2[] SnapableLines { get; } = new ParametricLine2[1] { new((0, 0), (1, 0)) };

        public void Draw(GeometryDrawer gd)
        {
            //Axis
            gd.DrawLine(axis, double.NegativeInfinity, double.PositiveInfinity, Style.Default);
            //Zero mark
            gd.DrawLine((0, -.2), (0, .2), Style.Default);
            gd.DrawString("0", (0.1, 0.1), 24, Style.Default);
            //Axis label
            Vector2 end = gd.DrawString("x", (5, 0.1), 24, Style.Default);
            gd.DrawString("1, 2", (end.X, .45), 16, Style.Default);
        }
    }
}
