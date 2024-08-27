using DegCAD.GeometryCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD.TimelineElements
{
    public interface IModification : ITimelineElement
    {
        public void Apply(Timeline tl);
        public void Remove(Timeline tl);
        public int CmdIndex { get; }
        public int ItemIndex { get; }
    }
}
