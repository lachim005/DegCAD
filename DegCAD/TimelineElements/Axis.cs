using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace DegCAD.TimelineElements
{
    /// <summary>
    /// Static class that can display the x axis and the zero mark
    /// </summary>
    public class Axis : ITimelineElement
    {
        public ITimelineElement Clone() => new Axis();
    }
}
