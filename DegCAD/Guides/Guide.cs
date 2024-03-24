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
        public int LastViewedStep { get; set; } = 0;
        public int LastEditedStep { get; set; } = 0;

        public Guide Clone()
        {
            Guide g = new() 
            {
                LastViewedStep = LastViewedStep,
                LastEditedStep = LastEditedStep
            };
            foreach (GuideStep step in Steps)
            {
                g.Steps.Add(step.Clone());
            }
            return g;
        }
    }
}
