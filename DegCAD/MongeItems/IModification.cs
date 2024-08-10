using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD.MongeItems
{
    public interface IModification : ITimelineElement
    {
        public void Apply(Timeline tl);
        public void Remove(Timeline tl);
    }
}
