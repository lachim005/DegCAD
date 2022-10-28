using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public class Snapper
    {
        public Timeline Timeline { get; init; }

        public Snapper(Timeline timeline)
        {
            Timeline = timeline;
        }

        public Vector2 Snap(Vector2 v)
        {
            return v;
        }
    }
}
