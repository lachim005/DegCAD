using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.Guides
{
    public class Guide
    {
        public ObservableCollection<GuideStep> Steps { get; init; } = new();
    }
}
